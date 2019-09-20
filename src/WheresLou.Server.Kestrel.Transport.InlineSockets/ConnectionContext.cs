using System.Buffers;
using System.Net.Sockets;
using System.Threading;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public struct ConnectionContext 
    {
        public ConnectionContext(
            MemoryPool<byte> memoryPool, 
            Socket socket,
            CancellationToken connectionClosed)
        {
            MemoryPool = memoryPool;
            Socket = socket;
            ConnectionClosed = connectionClosed;
        }

        public MemoryPool<byte> MemoryPool { get; }
        public Socket Socket { get; }
        public CancellationToken ConnectionClosed { get; }
    }
}
