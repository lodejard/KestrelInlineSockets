// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Tests.Fixtures
{
    public class EndPointFixture
    {
        public IPEndPoint IPEndPoint { get; set; } 

        public void FindUnusedPort()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));

            IPEndPoint = (IPEndPoint)socket.LocalEndPoint;
        }
    }
}
