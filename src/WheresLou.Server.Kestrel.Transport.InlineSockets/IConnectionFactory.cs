using System.IO.Pipelines;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Internals;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Factories
{
    public interface IConnectionFactory
    {
        IConnection CreateConnection(INetworkSocket socket);
        PipeReader CreatePipeReader(IConnection connection, INetworkSocket socket);
        PipeWriter CreatePipeWriter(IConnection connection, INetworkSocket socket);
    }
}
