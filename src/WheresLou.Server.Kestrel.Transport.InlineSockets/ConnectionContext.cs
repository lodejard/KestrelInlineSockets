using System.Buffers;
using System.Net.Sockets;
using System.Threading;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Internals;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public struct ConnectionContext 
    {
        public ConnectionContext(
            MemoryPool<byte> memoryPool, 
            INetworkSocket socket,
            CancellationTokenSource connectionClosed)
        {
            MemoryPool = memoryPool;
            Socket = socket;
            ConnectionClosed = connectionClosed;
        }

        public MemoryPool<byte> MemoryPool { get; }
        public INetworkSocket Socket { get; }
        public CancellationTokenSource ConnectionClosed { get; }
    }
}
