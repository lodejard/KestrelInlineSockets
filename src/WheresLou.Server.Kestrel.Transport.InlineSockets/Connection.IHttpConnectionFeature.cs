// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Net;
using Microsoft.AspNetCore.Http.Features;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public partial class Connection : IHttpConnectionFeature
    {
        string IHttpConnectionFeature.ConnectionId
        {
            get => _connectionId;
            set => _connectionId = value;
        }

        IPAddress IHttpConnectionFeature.RemoteIpAddress
        {
            get => _socketRemoteEndPoint.Address;
            set => throw new NotImplementedException();
        }

        IPAddress IHttpConnectionFeature.LocalIpAddress
        {
            get => _socketLocalEndPoint.Address;
            set => throw new NotImplementedException();
        }

        int IHttpConnectionFeature.RemotePort
        {
            get => _socketRemoteEndPoint.Port;
            set => throw new NotImplementedException();
        }

        int IHttpConnectionFeature.LocalPort
        {
            get => _socketLocalEndPoint.Port;
            set => throw new NotImplementedException();
        }
    }
}
