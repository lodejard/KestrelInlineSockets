using System.IO.Pipelines;
using Microsoft.Extensions.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Factories
{
    public class ConnectionPipeWriterFactory : IConnectionPipeWriterFactory
    {
        private readonly ILogger<ConnectionPipeWriter> _logger;

        public ConnectionPipeWriterFactory(ILogger<ConnectionPipeWriter> logger)
        {
            _logger = logger;
        }

        public virtual PipeWriter Create(ConnectionContext context)
        {
            return new ConnectionPipeWriter(_logger, context);
        }
    }
}
