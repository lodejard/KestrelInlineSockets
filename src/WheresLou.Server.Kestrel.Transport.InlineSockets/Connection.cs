// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO.Pipelines;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Network;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public partial class Connection : IConnection
    {
        private readonly IFeatureCollection _features;
        private readonly IConnectionLogger _logger;
        private readonly InlineSocketsOptions _options;
        private readonly INetworkSocket _socket;
        private readonly PipeReader _connectionPipeReader;
        private readonly PipeWriter _connectionPipeWriter;
        private readonly IPEndPoint _socketRemoteEndPoint;
        private readonly IPEndPoint _socketLocalEndPoint;
        private readonly CancellationTokenSource _connectionClosedTokenSource;

        private string _connectionId;
        private IDuplexPipe _transport;
        private object _synchronizeCompletion = new object();
        private bool _pipeWriterComplete;
        private bool _pipeReaderComplete;

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

            // preserving these values to avoid errors once the socket is closed
            _socketRemoteEndPoint = _socket.RemoteEndPoint;
            _socketLocalEndPoint = _socket.LocalEndPoint;
        }

        public IFeatureCollection Features => _features;

        void IConnection.Abort(ConnectionAbortedException abortReason)
        {
            OnAbortRequested(abortReason);
        }

        void IDisposable.Dispose()
        {
            _logger.LogDebug("TODO: Dispose {ConnectionId}", _connectionId);

            (_connectionPipeReader as IDisposable)?.Dispose();
            (_connectionPipeWriter as IDisposable)?.Dispose();
            _socket.Dispose();
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            ((IDisposable)this).Dispose();
        }

        void IConnection.OnPipeReaderComplete(Exception exception)
        {
            OnPipeComplete(pipeReaderComplete: true);
        }

        void IConnection.OnPipeWriterComplete(Exception exception)
        {
            OnPipeComplete(pipeWriterComplete: true);
        }

        private void OnPipeComplete(
            bool pipeReaderComplete = false,
            bool pipeWriterComplete = false)
        {
            var connectionClosed = false;
            var readerRemaining = false;
            lock (_synchronizeCompletion)
            {
                if (pipeReaderComplete)
                {
                    _pipeReaderComplete = true;
                }

                if (pipeWriterComplete)
                {
                    _pipeWriterComplete = true;
                }

                connectionClosed = _pipeReaderComplete && _pipeWriterComplete;
                readerRemaining = (_pipeReaderComplete == false) && _pipeWriterComplete;
            }

            if (connectionClosed)
            {
                // signal all tranceiving is complete
                _connectionClosedTokenSource.Cancel(throwOnFirstException: false);
            }
            else if (readerRemaining)
            {
                // this is necessary for Kestrel to realize the connection has ended
                _connectionPipeReader.CancelPendingRead();
            }
        }

        private void OnAbortRequested(ConnectionAbortedException abortReason)
        {
            _logger.LogDebug("TODO: AbortRequested {ConnectionId}", _connectionId);

            // stop any additional data from arriving
            _connectionPipeReader.CancelPendingRead();
        }
    }
}
