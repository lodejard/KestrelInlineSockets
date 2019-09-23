using System.Buffers;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Extensions.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Internals;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public struct ConnectionContext<TCategoryName>
    {
        public ConnectionContext(
            ILogger<TCategoryName> logger,
            InlineSocketsTransportOptions options)
        {
            Logger = logger;
            Options = options;
        }

        public ILogger<TCategoryName> Logger{get;}
        public InlineSocketsTransportOptions Options { get; }
    }
}
