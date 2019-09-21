using System;
using System.Buffers;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Internals
{
    public interface INetworkSocket
    {
        EndPoint LocalEndPoint { get; }
        EndPoint RemoteEndPoint { get; }

        int Send(ReadOnlySequence<byte> buffers);
        Task<int> ReceiveAsync(Memory<byte> buffers, CancellationToken cancellationToken);
        void CancelPendingRead();
    }
}
