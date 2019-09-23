using System;
using System.Threading.Tasks;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Internals
{
    public interface INetworkListener : IDisposable
    {
        void Start();
        void Stop();
        Task<INetworkSocket> AcceptSocketAsync();
    }
}
