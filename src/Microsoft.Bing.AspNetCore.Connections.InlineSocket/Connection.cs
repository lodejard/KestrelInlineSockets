// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO.Pipelines;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network;
using Microsoft.Extensions.Logging;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public partial class Connection : IConnection
    {
        private readonly IFeatureCollection _features;
        private readonly IConnectionLogger _logger;
        private readonly InlineSocketsOptions _options;
        private readonly INetworkSocket _socket;
        private readonly PipeReader _connectionPipeReader;
        private readonly PipeWriter _connectionPipeWriter;
        private readonly CancellationTokenSource _connectionClosedTokenSource;

        private string _connectionId;
        private EndPoint _remoteEndPoint;
        private EndPoint _localEndPoint;
        private IDuplexPipe _transport;

        public Connection(
            IConnectionLogger logger,
            InlineSocketsOptions options,
            INetworkSocket socket)
        {
            _features = new FeatureCollection(this);
            _logger = logger;
            _options = options;
            _socket = socket;
            _transport = this;
            _connectionPipeReader = options.CreatePipeReader(this, socket);
            _connectionPipeWriter = options.CreatePipeWriter(this, socket);

            _connectionClosedTokenSource = new CancellationTokenSource();
            _connectionClosedTokenSource.Token.Register(() => _logger.LogTrace("TODO: ConnectionClosed"));

            _remoteEndPoint = _socket.RemoteEndPoint;
            _localEndPoint = _socket.LocalEndPoint;
        }

        public IFeatureCollection Features => _features;

        public virtual string ConnectionId
        {
            get => _connectionId ?? Interlocked.CompareExchange(ref _connectionId, CorrelationIdGenerator.GetNextId(), null) ?? _connectionId;
            set => _connectionId = value;
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            _logger.LogDebug("TODO: DisposeAsync {ConnectionId}", ConnectionId);

            await _socket.DisposeAsync();

            ((IDisposable)this).Dispose();
        }

        void IDisposable.Dispose()
        {
            _logger.LogDebug("TODO: Dispose {ConnectionId}", ConnectionId);

            (_connectionPipeReader as IDisposable)?.Dispose();
            (_connectionPipeWriter as IDisposable)?.Dispose();
            _socket.Dispose();

            _connectionClosedTokenSource.Dispose();
#if NETSTANDARD2_0
            _connectionCloseRequestedSource.Dispose();
#endif
        }

        private void OnAbortRequested(ConnectionAbortedException abortReason)
        {
            _logger.LogDebug(abortReason, "TODO: AbortRequested {ConnectionId}", ConnectionId);

            // immediate FIN so client understands server will not complete current response or accept subsequent requests
            _socket.ShutdownSend();

            // stop any additional data from arriving
            _connectionPipeReader.CancelPendingRead();
        }
    }
}
