// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO.Pipelines;
using Microsoft.Extensions.Options;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Internals;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly ConnectionContext _context;

        public ConnectionFactory(
            IConnectionLogger connectionLogger,
            IOptions<InlineSocketsTransportOptions> options)
        {
            _context = new ConnectionContext(
                connectionLogger, 
                options.Value);
        }

        public virtual IConnection CreateConnection(
            INetworkSocket socket)
        {
            return new Connection(
                _context,
                this,
                socket);
        }

        public virtual PipeReader CreatePipeReader(
            IConnection connection,
            INetworkSocket socket)
        {
            return new ConnectionPipeReader(
                _context,
                connection,
                socket);
        }

        public virtual PipeWriter CreatePipeWriter(
            IConnection connection,
            INetworkSocket socket)
        {
            return new ConnectionPipeWriter(
                _context,
                connection,
                socket);
        }
    }
}
