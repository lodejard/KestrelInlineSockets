// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.AspNetCore.Connections.Features;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public partial class Connection : IConnectionHeartbeatFeature
    {
        void IConnectionHeartbeatFeature.OnHeartbeat(Action<object> action, object state)
        {
            // ignored
        }
    }
}
