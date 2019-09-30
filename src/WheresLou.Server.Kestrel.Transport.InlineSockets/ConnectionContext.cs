// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public struct ConnectionContext
    {
        public ConnectionContext(
            IConnectionLogger logger,
            InlineSocketsTransportOptions options)
        {
            Logger = logger;
            Options = options;
        }

        public IConnectionLogger Logger { get; }

        public InlineSocketsTransportOptions Options { get; }
    }
}
