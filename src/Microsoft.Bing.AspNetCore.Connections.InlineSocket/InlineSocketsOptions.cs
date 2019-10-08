// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Buffers;
using System.IO.Pipelines;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public class InlineSocketsOptions
    {
        public MemoryPool<byte> MemoryPool { get; set; }

        public int? ListenBacklog { get; set; }

        public bool? AllowNatTraversal { get; set; }

        public bool? ExclusiveAddressUse { get; set; }

        public bool? NoDelay { get; set; }

        public Func<IListener> CreateListener { get; set; }

        public Func<INetworkSocket, IConnection> CreateConnection { get; set; }

        public Func<IConnection, INetworkSocket, PipeReader> CreatePipeReader { get; set; }

        public Func<IConnection, INetworkSocket, PipeWriter> CreatePipeWriter { get; set; }
    }
}
