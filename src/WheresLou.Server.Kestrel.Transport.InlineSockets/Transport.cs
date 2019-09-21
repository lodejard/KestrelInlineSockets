using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Factories;
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
            _acceptLoopTokenSource.Cancel();
            _listener.Stop();
            await _acceptLoopTask;
        }

        public Task StopAsync()
        {
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

                    var task = Task.Run(() => ProcessSocketAsync(socket, cancellationToken));

                    // TODO: need better way to ensure pending tasks complete before this method returns?
                    HandleErrors(task);
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
                {
                    // listen socket closed
                }
                catch (ObjectDisposedException)
                {
                    // listen socket closed
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

        public async Task ProcessSocketAsync(INetworkSocket socket, CancellationToken cancellationToken)
        {
            var connectionClosedTokenSource = new CancellationTokenSource();
            try
            {
                var connectionContext = new ConnectionContext(
                    _context.MemoryPool,
                    socket,
                    connectionClosedTokenSource);

                var connection = _context.ConnectionFactory.Create(connectionContext);

                // 1. get onconnection incomplete task
                var dispatcherTask = _connectionDispatcher.OnConnection(connection.TransportConnection);

                try
                {
                    // 2. get tranceiving incomplete task
                    // 3. await tranceiving task
                    // await connection.TranceiveAsync();
                }
                finally
                {
                    // 4. await onconnection task
                    await dispatcherTask;
                    Console.WriteLine("dispatcherTask completed");
                }
            }
            finally
            {
                // 5. dispose connection
                connectionClosedTokenSource.Dispose();
            }
        }
    }
}
