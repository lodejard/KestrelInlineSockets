// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Buffers;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Memory
{
    public class RollingMemorySegment : ReadOnlySequenceSegment<byte>, IDisposable
    {
        private IMemoryOwner<byte> _rental;

        public RollingMemorySegment(IMemoryOwner<byte> rental, long runningIndex, long runningOrdinal)
        {
            _rental = rental;
            Memory = _rental.Memory;
            RunningIndex = runningIndex;
        }

        public new RollingMemorySegment Next
        {
            get => (RollingMemorySegment)base.Next;
            set => base.Next = value;
        }

        public int RunningOrdinal { get; set; }

        public void Dispose()
        {
            _rental.Dispose();
        }
    }
}
