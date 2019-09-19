using System.Buffers;
using System.Net.Sockets;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public struct ConnectionContext 
    {
        public ConnectionContext(
            MemoryPool<byte> memoryPool, 
            Socket socket)
        {
            MemoryPool = memoryPool;
            Socket = socket;
        }

        public MemoryPool<byte> MemoryPool { get; }
        public Socket Socket { get; }
    }
}
