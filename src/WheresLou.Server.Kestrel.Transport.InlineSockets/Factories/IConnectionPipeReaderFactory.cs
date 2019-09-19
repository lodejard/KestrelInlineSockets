using System.IO.Pipelines;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Factories
{
    public interface IConnectionPipeReaderFactory
    {
        PipeReader Create(ConnectionContext context);
    }
}
