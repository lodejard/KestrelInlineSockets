using System.Net.Sockets;
using System.Threading.Tasks;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Internals
{
    public class NetworkListener : INetworkListener
    {
        private readonly TcpListener _listener;
        private readonly int? _listenerBacklog;
        private readonly bool? _socketNoDelay;

        public NetworkListener(NetworkListenerSettings settings)
        {
            // TODO: logic to bind ipv4 and/or ipv6 ?
            _listener = new TcpListener(settings.EndPointInformation.IPEndPoint);

            if (settings.ExclusiveAddressUse.HasValue)
            {
                _listener.ExclusiveAddressUse = settings.ExclusiveAddressUse.Value;
            }

            if (settings.AllowNatTraversal.HasValue)
            {
                _listener.AllowNatTraversal(settings.AllowNatTraversal.Value);
            }

            _listenerBacklog = settings.ListenerBacklog;
            _socketNoDelay = settings.EndPointInformation.NoDelay;
        }

        public virtual void Dispose()
        {
            _listener.Stop();
        }

        public virtual void Start()
        {
            if (_listenerBacklog.HasValue)
            {
                _listener.Start(_listenerBacklog.Value);
            }
            else
            {
                _listener.Start();
            }
        }

        public virtual void Stop()
        {
            _listener.Stop();
        }

        public virtual async Task<INetworkSocket> AcceptSocketAsync()
        {
            var socket = await _listener.AcceptSocketAsync();
            if (_socketNoDelay.HasValue)
            {
                socket.NoDelay = _socketNoDelay.Value;
            }
            return new NetworkSocket(socket);
        }
    }
}
