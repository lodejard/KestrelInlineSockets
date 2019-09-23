// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO.Pipelines;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Internals;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly ILogger<Connection> _connectionLogger;
        private readonly ILogger<ConnectionPipeReader> _connectionPipeReaderLogger;
        private readonly ILogger<ConnectionPipeWriter> _connectionPipeWriterLogger;
        private readonly IOptions<InlineSocketsTransportOptions> _options;

        public ConnectionFactory(
            ILogger<Connection> connectionLogger,
            ILogger<ConnectionPipeReader> connectionPipeReaderLogger,
            ILogger<ConnectionPipeWriter> connectionPipeWriterLogger,
            IOptions<InlineSocketsTransportOptions> options)
        {
            _connectionLogger = connectionLogger;
            _connectionPipeReaderLogger = connectionPipeReaderLogger;
            _connectionPipeWriterLogger = connectionPipeWriterLogger;
            _options = options;
        }

        public virtual IConnection CreateConnection(
            INetworkSocket socket)
        {
            return new Connection(
                new ConnectionContext<Connection>(
                    _connectionLogger,
                    _options.Value),
                this,
                socket);
        }

        public virtual PipeReader CreatePipeReader(
            IConnection connection,
            INetworkSocket socket)
        {
            return new ConnectionPipeReader(
                new ConnectionContext<ConnectionPipeReader>(
                    _connectionPipeReaderLogger,
                    _options.Value),
                connection,
                socket);
        }

        public virtual PipeWriter CreatePipeWriter(
            IConnection connection,
            INetworkSocket socket)
        {
            return new ConnectionPipeWriter(
                new ConnectionContext<ConnectionPipeWriter>(
                    _connectionPipeWriterLogger,
                    _options.Value),
                connection,
                socket);
        }
    }
}
