// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if NETSTANDARD2_0
using System;
using System.Net;
using Microsoft.AspNetCore.Http.Features;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public partial class Connection : IHttpConnectionFeature
    {
        string IHttpConnectionFeature.ConnectionId
        {
            get => ConnectionId;
            set => ConnectionId = value;
        }

        IPAddress IHttpConnectionFeature.RemoteIpAddress
        {
            get => ((IPEndPoint)_remoteEndPoint).Address;
            set => throw new NotImplementedException();
        }

        IPAddress IHttpConnectionFeature.LocalIpAddress
        {
            get => ((IPEndPoint)_localEndPoint).Address;
            set => throw new NotImplementedException();
        }

        int IHttpConnectionFeature.RemotePort
        {
            get => ((IPEndPoint)_remoteEndPoint).Port;
            set => throw new NotImplementedException();
        }

        int IHttpConnectionFeature.LocalPort
        {
            get => ((IPEndPoint)_localEndPoint).Port;
            set => throw new NotImplementedException();
        }
    }
}
#endif
