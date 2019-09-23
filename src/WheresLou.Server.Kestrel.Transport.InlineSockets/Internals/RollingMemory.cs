// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class RollingMemory
    {
        private readonly MemoryPool<byte> _memoryPool;
        private RollingMemorySegment _firstSegment;
        private int _firstIndex;
        private RollingMemorySegment _lastSegment;
        private int _lastIndex;

        public RollingMemory(MemoryPool<byte> memoryPool)
        {
            _memoryPool = memoryPool;
        }

        public bool IsEmpty => _firstSegment == _lastSegment && _firstIndex == _lastIndex;

        public ReadOnlySequence<byte> GetOccupiedMemory()
        {
            return new ReadOnlySequence<byte>(_firstSegment, _firstIndex, _lastSegment, _lastIndex);
        }

        public void ConsumeOccupiedMemory(SequencePosition consumed)
        {
            var consumedObject = consumed.GetObject();
            var consumedInteger = consumed.GetInteger();
            while (consumedObject != _firstSegment)
            {
                DisposeFirstSegment();
            }

            _firstIndex = consumedInteger;

            if (_firstSegment != _lastSegment && _firstIndex == _firstSegment.Memory.Length)
            {
                DisposeFirstSegment();
            }
        }

        public void ConsumeOccupiedMemory(int consumed)
        {
            var remaining = consumed;
            while (remaining != 0)
            {
                var occupied = _firstSegment.Memory.Length - _firstIndex;
                if (remaining < occupied)
                {
                    _firstIndex += remaining;
                    break;
                }

                DisposeFirstSegment();
                remaining -= occupied;
            }

            if (_firstSegment != _lastSegment && _firstIndex == _firstSegment.Memory.Length)
            {
                DisposeFirstSegment();
            }
        }

        public void DisposeFirstSegment()
        {
            var consumedSegment = _firstSegment;
            _firstSegment = consumedSegment.Next;
            _firstIndex = 0;
            consumedSegment.Dispose();
        }

        public Memory<byte> GetTrailingMemory(int sizeHint = 0)
        {
            // special case, no buffers.
            // allocate buffer on first read.
            if (_firstSegment == null && _lastSegment == null)
            {
                var rental = _memoryPool.Rent(sizeHint);
                var segment = new RollingMemorySegment(rental, 0, 0);
                _firstSegment = segment;
                _lastSegment = segment;
                return rental.Memory;
            }

            // special case, all occupied memory has been consumed.
            // drop both index to 0 so the entire page becomes trailing memory again.
            if (IsEmpty)
            {
                _firstIndex = 0;
                _lastIndex = 0;
            }

            // special case, last page is completely full.
            // allocate and append a new unoccupied last page.
            if (_lastIndex == _lastSegment.Memory.Length)
            {
                var rental = _memoryPool.Rent(sizeHint);
                _lastSegment.Next = new RollingMemorySegment(
                    rental,
                    _lastSegment.RunningIndex + _lastSegment.Memory.Length,
                    _lastSegment.RunningOrdinal + 1);
                _lastSegment = _lastSegment.Next;
                _lastIndex = 0;
                return rental.Memory;
            }

            return MemoryMarshal.AsMemory(_lastSegment.Memory.Slice(_lastIndex));
        }

        public bool HasUnexaminedData(SequencePosition examined)
        {
            if (IsEmpty)
            {
                return false;
            }

            var examinedObject = examined.GetObject();
            var examinedInteger = examined.GetInteger();
            if (ReferenceEquals(examinedObject, _lastSegment) &&
                examinedInteger == _lastIndex)
            {
                return false;
            }

            return true;
        }

        public void ConsumeTrailingMemory(int consumed)
        {
            _lastIndex += consumed;
        }

        public void TrailingMemoryFilled(int bytesReceived)
        {
            _lastIndex += bytesReceived;
        }
    }
}
