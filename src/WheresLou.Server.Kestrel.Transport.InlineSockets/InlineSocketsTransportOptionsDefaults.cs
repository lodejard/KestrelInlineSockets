// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Options;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class InlineSocketsTransportOptionsDefaults : IConfigureOptions<InlineSocketsTransportOptions>
    {
        public void Configure(InlineSocketsTransportOptions options)
        {
            options.MemoryPool = KestrelMemoryPool.Create();
        }
    }
}
