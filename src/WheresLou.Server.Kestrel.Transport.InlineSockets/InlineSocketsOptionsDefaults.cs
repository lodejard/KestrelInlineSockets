// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Buffers;
using System.IO.Pipelines;
using Microsoft.Extensions.Options;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Network;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class InlineSocketsOptionsDefaults : IConfigureOptions<InlineSocketsOptions>
    {
        private readonly Action<InlineSocketsOptions> _configure;

        public InlineSocketsOptionsDefaults(
            IListenerLogger listenerLogger,
            IConnectionLogger connectionLogger,
            INetworkProvider networkProvider)
        {
            _configure = options =>
            {
#if NETSTANDARD2_0
                var memoryPool = Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal.KestrelMemoryPool.Create();
#else
                var memoryPool = MemoryPool<byte>.Shared;
#endif

                options.MemoryPool = memoryPool;
                options.CreateListener = CreateListener;
                options.CreateConnection = CreateConnection;
                options.CreatePipeReader = CreatePipeReader;
                options.CreatePipeWriter = CreatePipeWriter;

                Listener CreateListener()
                {
                    return new Listener(listenerLogger, options, networkProvider);
                }

                IConnection CreateConnection(INetworkSocket socket)
                {
                    return new Connection(connectionLogger, options, socket);
                }

                PipeReader CreatePipeReader(IConnection connection, INetworkSocket socket)
                {
                    return new ConnectionPipeReader(connectionLogger, options, connection, socket);
                }

                PipeWriter CreatePipeWriter(IConnection connection, INetworkSocket socket)
                {
                    return new ConnectionPipeWriter(connectionLogger, options, connection, socket);
                }
            };
        }

        public void Configure(InlineSocketsOptions options) => _configure(options);
    }
}
