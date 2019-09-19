using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using System.Buffers;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public struct TransportContext
    {
        public TransportContext(
            MemoryPool<byte> memoryPool, 
            IEndPointInformation endPointInformation, 
            IConnectionDispatcher connectionDispatcher) 
        {
            MemoryPool = memoryPool;
            EndPointInformation = endPointInformation;
            ConnectionDispatcher = connectionDispatcher;
        }

        public MemoryPool<byte> MemoryPool { get; }
        public IEndPointInformation EndPointInformation { get; }
        public IConnectionDispatcher ConnectionDispatcher { get; }
    }
}
