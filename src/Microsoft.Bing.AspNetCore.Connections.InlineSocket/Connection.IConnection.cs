// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Net;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public partial class Connection : IConnection
    {
        string IConnection.ConnectionId
        {
            get => ConnectionId;
            set => ConnectionId = value;
        }

        EndPoint IConnection.LocalEndPoint
        {
            get => _localEndPoint;
            set => _localEndPoint = value;
        }

        EndPoint IConnection.RemoteEndPoint
        {
            get => _remoteEndPoint;
            set => _remoteEndPoint = value;
        }

        IFeatureCollection IConnection.Features => Features;

        void IConnection.Abort(ConnectionAbortedException abortReason)
        {
            OnAbortRequested(abortReason);
        }

        void IConnection.FireConnectionClosed()
        {
            _connectionClosedTokenSource.Cancel();
        }
    }
}
