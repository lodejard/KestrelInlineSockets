using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class InlineSocketsTransportOptions
    {
        public MemoryPool<byte> MemoryPool{get;set;}
        public int? ListenBacklog { get; set; }
        public bool? AllowNatTraversal { get; set; }
        public bool? ExclusiveAddressUse { get; set; }
    }
}
