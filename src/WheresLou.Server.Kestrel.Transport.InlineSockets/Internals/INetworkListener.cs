using System.Threading.Tasks;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Internals
{
    public interface INetworkListener
    {
        void Start();
        void Stop();
        Task<INetworkSocket> AcceptSocketAsync();
    }
}
