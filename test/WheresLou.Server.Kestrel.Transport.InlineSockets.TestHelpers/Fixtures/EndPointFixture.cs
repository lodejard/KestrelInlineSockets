// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Net;
using System.Net.Sockets;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Stubs;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Fixtures
{
    public class EndPointFixture : IDisposable
    {
#if NETSTANDARD2_0
        public TestEndPointInformation EndPointInformation { get; set; } = new TestEndPointInformation();

        public void FindUnusedPort()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP))
            {
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));

                EndPointInformation.Type = Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal.ListenType.IPEndPoint;
                EndPointInformation.IPEndPoint = (IPEndPoint)socket.LocalEndPoint;

                socket.Close();
            }
        }
#endif

        public void Dispose()
        {
        }
    }
}
