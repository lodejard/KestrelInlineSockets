using System.Buffers;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Factories;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class TransportFactory : ITransportFactory
    {
        private readonly ILogger<Transport> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private readonly MemoryPool<byte> _memoryPool;

        public TransportFactory(
            ILogger<Transport> logger,
            IConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
            _memoryPool = KestrelMemoryPool.Create();
        }

        public virtual ITransport Create(
            IEndPointInformation endPointInformation, 
            IConnectionDispatcher connectionDispatcher)
        {
            return new Transport(
                _logger, 
                _connectionFactory, 
                new TransportContext(
                    _memoryPool, 
                    endPointInformation, 
                    connectionDispatcher));
        }
    }
}
