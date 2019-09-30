// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Internals;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class Transport : ITransport
    {
        private readonly TransportContext _context;
        private readonly IEndPointInformation _endPointInformation;
        private readonly IConnectionDispatcher _connectionDispatcher;

        private INetworkListener _listener;
        private CancellationTokenSource _acceptLoopTokenSource;
        private Task _acceptLoopTask;

        public Transport(
            TransportContext context,
            IEndPointInformation endPointInformation,
            IConnectionDispatcher connectionDispatcher)
        {
            _context = context;
            _endPointInformation = endPointInformation;
            _connectionDispatcher = connectionDispatcher;
        }

        public Task BindAsync()
        {
            _context.Logger.LogDebug(new EventId(1, "BindListenSocket"), "Binding listen socket to {IPEndPoint}", _endPointInformation.IPEndPoint);

            _listener = _context.NetworkProvider.CreateListener(new NetworkListenerSettings
            {
                EndPointInformation = _endPointInformation,
                AllowNatTraversal = _context.Options.AllowNatTraversal,
                ExclusiveAddressUse = _context.Options.ExclusiveAddressUse,
                ListenerBacklog = _context.Options.ListenBacklog,
            });

            _listener.Start();

            _acceptLoopTokenSource = new CancellationTokenSource();
            _acceptLoopTask = Task.Run(() => AcceptLoopAsync(_listener, _acceptLoopTokenSource.Token));

            return Task.CompletedTask;
        }

        public async Task UnbindAsync()
        {
            _context.Logger.LogDebug(new EventId(2, "UnbindListenSocket"), "Unbinding listen socket from {IPEndPoint}", _endPointInformation.IPEndPoint);

            _acceptLoopTokenSource.Cancel(throwOnFirstException: false);
            _listener.Stop();
            await _acceptLoopTask;

            _listener.Dispose();
            _listener = null;
        }

        public Task StopAsync()
        {
            _context.Logger.LogDebug(new EventId(3, "StopTransport"), "Inline sockets transport is stopped.");
            return Task.CompletedTask;
        }

        public async Task AcceptLoopAsync(INetworkListener listener, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var socket = await listener.AcceptSocketAsync();

                    _context.Logger.LogInformation(new EventId(5, "SocketAccepted"), "Socket accepted from {RemoteEndPoint} to {LocalEndPoint}", socket.RemoteEndPoint, socket.LocalEndPoint);

                    var task = ProcessSocketAsyncSafe(socket, cancellationToken);

                    // TODO: need better way to ensure pending tasks complete before this method returns?
                    // for now, fire-and-forget async method will at least observe and log exceptions
                    HandleErrors(task);
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
                {
                    // normal exception: listen socket closed
                }
                catch (ObjectDisposedException)
                {
                    // normal exception: listen socket closed
                }
            }
        }

        public async void HandleErrors(Task task)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                _context.Logger.LogError(new EventId(6, "OnConnectionError"), ex, "Unexpected failure thrown from IConnectionDispatcher.OnConnection");
            }
        }

        public Task ProcessSocketAsyncSafe(INetworkSocket socket, CancellationToken cancellationToken)
        {
            try
            {
                return ProcessSocketAsync(socket, cancellationToken);
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        public async Task ProcessSocketAsync(INetworkSocket socket, CancellationToken cancellationToken)
        {
            var connection = _context.ConnectionFactory.CreateConnection(socket);
            try
            {
                await _connectionDispatcher.OnConnection(connection.TransportConnection);
            }
            finally
            {
                connection.Dispose();
            }
        }
    }
}
