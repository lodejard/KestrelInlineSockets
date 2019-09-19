using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public interface IConnectionFactory
    {
        TransportConnection Create(ConnectionContext connectionContext);
    }
}
