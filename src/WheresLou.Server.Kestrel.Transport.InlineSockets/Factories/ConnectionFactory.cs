using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Internals;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Factories
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly ILogger<Connection> _logger;
        private readonly INetworkProvider _networkProvider;
        private readonly IConnectionPipeReaderFactory _connectionPipeReaderFactory;
        private readonly IConnectionPipeWriterFactory _connectionPipeWriterFactory;

        public ConnectionFactory(
            ILogger<Connection> logger,
            INetworkProvider networkProvider,
            IConnectionPipeReaderFactory connectionPipeReaderFactory,
            IConnectionPipeWriterFactory connectionPipeWriterFactory)
        {
            _logger = logger;
            _networkProvider = networkProvider;
            _connectionPipeReaderFactory = connectionPipeReaderFactory;
            _connectionPipeWriterFactory = connectionPipeWriterFactory;
        }

        public virtual IConnection Create(ConnectionContext context)
        {
            return new Connection(
                _logger,
                _networkProvider,
                _connectionPipeReaderFactory,
                _connectionPipeWriterFactory,
                context);
        }
    }
}
