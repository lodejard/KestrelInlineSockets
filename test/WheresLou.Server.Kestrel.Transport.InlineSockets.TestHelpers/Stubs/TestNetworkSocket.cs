// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Buffers;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Network;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Stubs
{
    public class TestNetworkSocket : INetworkSocket
    {
        public IPEndPoint LocalEndPoint { get; set; }

        public IPEndPoint RemoteEndPoint { get; set; }

        public bool IsDisposed { get; set; }

        public void Dispose() => IsDisposed = true;

        public void CancelPendingRead()
        {
            throw new NotImplementedException();
        }

        public Task<int> ReceiveAsync(Memory<byte> buffers, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public int Send(ReadOnlySequence<byte> buffers)
        {
            throw new NotImplementedException();
        }

        public void ShutdownSend()
        {
            throw new NotImplementedException();
        }
    }
}
