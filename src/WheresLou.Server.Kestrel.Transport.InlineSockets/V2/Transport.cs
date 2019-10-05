// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Options;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Network;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class Transport : ITransport
    {
        private readonly Listener _listener;
        private readonly IListenerLogger _logger;
        private readonly IEndPointInformation _endPointInformation;
        private readonly IConnectionDispatcher _dispatcher;
        private readonly CancellationTokenSource _acceptLoopCancellation = new CancellationTokenSource();
        private Task _acceptLoopTask;

        public Transport(
            IListenerLogger logger,
            InlineSocketsOptions options,
            INetworkProvider networkProvider,
            IEndPointInformation endPointInformation,
            IConnectionDispatcher dispatcher)
        {
            _listener = new Listener(logger, options, networkProvider);
            _logger = logger;
            _endPointInformation = endPointInformation;
            _dispatcher = dispatcher;
        }

        public virtual async Task BindAsync()
        {
            await _listener.BindAsync(
                _endPointInformation.IPEndPoint,
                _endPointInformation.NoDelay);

            _acceptLoopTask = Task.Run(() => AcceptLoopAsync(_acceptLoopCancellation.Token));
        }

        public virtual async Task UnbindAsync()
        {
            _acceptLoopCancellation.Cancel();

            try
            {
                await _acceptLoopTask;
            }
            catch (TaskCanceledException)
            {
                // normal exit from cancellation
            }

            await _listener.UnbindAsync();
        }

        public virtual async Task StopAsync()
        {
            await _listener.DisposeAsync();
        }

        public virtual async Task AcceptLoopAsync(CancellationToken cancellationToken = default)
        {
            for (; ; )
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var connection = await _listener.AcceptAsync(cancellationToken);
                    var task = DispatchConnectionAsync(connection);
                    HandleExceptions(task);
                }
                catch (ObjectDisposedException)
                {
                    // object disposed from listener means happens because the socket has been closed during cancellation
                    throw new TaskCanceledException();
                }
            }
        }

        private async Task DispatchConnectionAsync(IConnection connection)
        {
            try
            {
                var transportConnection = new TransportConnection(connection);
                await _dispatcher.OnConnection(transportConnection);
            }
            finally
            {
                await connection.DisposeAsync();
            }
        }

        private async void HandleExceptions(Task task)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                _logger.ConnectionDispatchFailed(ex);
            }
        }
    }
}
#endif
