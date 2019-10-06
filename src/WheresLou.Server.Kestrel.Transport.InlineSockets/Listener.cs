// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Network;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class Listener : IListener
    {
        private readonly IListenerLogger _logger;
        private readonly InlineSocketsOptions _options;
        private readonly INetworkProvider _networkProvider;
        private IPEndPoint _ipEndpoint;
        private INetworkListener _listener;
        private Action _listenerCancellationCallback;

        public Listener(
            IListenerLogger logger,
            InlineSocketsOptions options,
            INetworkProvider networkProvider)
        {
            _logger = logger;
            _options = options;
            _networkProvider = networkProvider;
        }

        public virtual EndPoint EndPoint => _ipEndpoint;

        public virtual async ValueTask BindAsync(EndPoint endpoint, bool? noDelay, CancellationToken cancellationToken = default)
        {
            if (endpoint is IPEndPoint ipEndpoint)
            {
                _ipEndpoint = ipEndpoint;
            }
            else
            {
                throw new NotSupportedException("Only IPEndPoint are currently supported by inline sockets.");
            }

            _logger.BindListenSocket(_ipEndpoint);

            _listener = _networkProvider.CreateListener(new NetworkListenerSettings
            {
                IPEndPoint = _ipEndpoint,
                AllowNatTraversal = _options.AllowNatTraversal,
                ExclusiveAddressUse = _options.ExclusiveAddressUse,
                ListenerBacklog = _options.ListenBacklog,
                NoDelay = noDelay,
            });

            // the only way to cancel a call to accept a socket is to stop the listener.
            // this is okay, because this call is cancelled only when the listener is about to be
            // unbound and disposed anyway.
            _listenerCancellationCallback = _listener.Stop;

            _listener.Start();
        }

        public virtual async ValueTask UnbindAsync(CancellationToken cancellationToken = default)
        {
            _logger.UnbindListenSocket(_ipEndpoint);
            _listener.Stop();
        }

        public virtual void Dispose()
        {
            _listener?.Dispose();
            _listener = null;
            _listenerCancellationCallback = null;
        }

        public virtual async ValueTask DisposeAsync()
        {
            Dispose();
        }

        public virtual async ValueTask<IConnection> AcceptAsync(CancellationToken cancellationToken = default)
        {
            using (cancellationToken.Register(_listenerCancellationCallback))
            {
                var socket = await _listener.AcceptSocketAsync();
                _logger.SocketAccepted(socket.RemoteEndPoint, socket.LocalEndPoint);
                return _options.CreateConnection(socket);
            }
        }
    }
}