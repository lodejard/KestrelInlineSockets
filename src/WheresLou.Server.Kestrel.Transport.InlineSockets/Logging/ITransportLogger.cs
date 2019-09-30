using System;
using System.Net;
using Microsoft.Extensions.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Logging
{
    public interface ITransportLogger : ILogger
    {
        void BindListenSocket(IPEndPoint ipEndPoint);

        void UnbindListenSocket(IPEndPoint ipEndPoint);

        void StopTransport();

        void SocketAccepted(EndPoint remoteEndPoint, EndPoint localEndPoint);

        void ConnectionDispatchFailed(Exception error);
    }
}
