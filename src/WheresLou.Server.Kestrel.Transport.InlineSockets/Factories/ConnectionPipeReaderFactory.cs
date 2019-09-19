using System.IO.Pipelines;
using Microsoft.Extensions.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Factories
{
    public class ConnectionPipeReaderFactory : IConnectionPipeReaderFactory
    {
        private readonly ILogger<ConnectionPipeReader> _logger;

        public ConnectionPipeReaderFactory(ILogger<ConnectionPipeReader> logger)
        {
            _logger = logger;
        }

        public virtual PipeReader Create(ConnectionContext context)
        {
            return new ConnectionPipeReader(_logger, context);
        }
    }
}
