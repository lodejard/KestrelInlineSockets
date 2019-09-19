using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Factories
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly ILogger<Connection> _logger;
        private readonly IConnectionPipeReaderFactory _connectionPipeReaderFactory;
        private readonly IConnectionPipeWriterFactory _connectionPipeWriterFactory;

        public ConnectionFactory(
            ILogger<Connection> logger,
            IConnectionPipeReaderFactory connectionPipeReaderFactory,
            IConnectionPipeWriterFactory connectionPipeWriterFactory)
        {
            _logger = logger;
            _connectionPipeReaderFactory = connectionPipeReaderFactory;
            _connectionPipeWriterFactory = connectionPipeWriterFactory;
        }

        public virtual TransportConnection Create(ConnectionContext context)
        {
            return new Connection(
                _logger,
                _connectionPipeReaderFactory,
                _connectionPipeWriterFactory,
                context);
        }
    }
}
