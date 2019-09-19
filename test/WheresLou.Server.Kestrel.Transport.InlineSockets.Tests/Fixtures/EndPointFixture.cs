using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Stubs;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Fixtures
{
    public class EndPointFixture : IDisposable
    {
        public TestEndPointInformation EndPointInformation { get; set; } = new TestEndPointInformation();

        public void Dispose()
        {
        }

        public void FindUnusedPort()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP))
            {
                //socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));

                EndPointInformation.Type = ListenType.IPEndPoint;
                EndPointInformation.IPEndPoint = (IPEndPoint)socket.LocalEndPoint;

                socket.Close();
            }
        }
    }
}
