using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public interface IConnection
    {
        TransportConnection TransportConnection { get; }

        Task TranceiveAsync();
    }
}
