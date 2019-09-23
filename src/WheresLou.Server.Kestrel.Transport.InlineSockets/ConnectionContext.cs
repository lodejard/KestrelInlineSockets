// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public struct ConnectionContext<TCategoryName>
    {
        public ConnectionContext(
            ILogger<TCategoryName> logger,
            InlineSocketsTransportOptions options)
        {
            Logger = logger;
            Options = options;
        }

        public ILogger<TCategoryName> Logger { get; }

        public InlineSocketsTransportOptions Options { get; }
    }
}
