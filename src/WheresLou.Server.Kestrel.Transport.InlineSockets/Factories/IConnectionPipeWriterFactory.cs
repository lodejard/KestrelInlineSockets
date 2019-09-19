using System.IO.Pipelines;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Factories
{
    public interface IConnectionPipeWriterFactory
    {
        PipeWriter Create(ConnectionContext context);
    }
}
