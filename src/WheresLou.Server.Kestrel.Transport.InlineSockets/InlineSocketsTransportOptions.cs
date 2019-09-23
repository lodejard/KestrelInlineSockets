// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Buffers;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class InlineSocketsTransportOptions
    {
        public MemoryPool<byte> MemoryPool { get; set; }

        public int? ListenBacklog { get; set; }

        public bool? AllowNatTraversal { get; set; }

        public bool? ExclusiveAddressUse { get; set; }
    }
}
