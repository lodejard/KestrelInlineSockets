// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Connections.Features;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public partial class Connection : IConnectionIdFeature
    {
        string IConnectionIdFeature.ConnectionId
        {
            get => _connectionId;
            set => _connectionId = value;
        }
    }
}
