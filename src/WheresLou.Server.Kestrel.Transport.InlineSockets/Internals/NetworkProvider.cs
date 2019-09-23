using System.Text;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Internals
{
    public class NetworkProvider : INetworkProvider
    {
        public virtual INetworkListener CreateListener(NetworkListenerSettings settings)
        {
            return new NetworkListener(settings);
        }
    }
}
