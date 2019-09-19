using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Factories
{
    public interface IConnectionFactory
    {
        TransportConnection Create(ConnectionContext context);
    }
}
