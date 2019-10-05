// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Buffers;
using Microsoft.AspNetCore.Connections.Features;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public partial class Connection : IMemoryPoolFeature
    {
        MemoryPool<byte> IMemoryPoolFeature.MemoryPool => _options.MemoryPool;
    }
}
