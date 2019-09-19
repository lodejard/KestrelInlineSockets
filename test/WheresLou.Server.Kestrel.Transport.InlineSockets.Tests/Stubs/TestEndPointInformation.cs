using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Stubs
{
    internal class TestEndPointInformation : IEndPointInformation
    {
        public ListenType Type { get; set; }

        public IPEndPoint IPEndPoint { get; set; }

        public string SocketPath { get; set; }

        public ulong FileHandle { get; set; }

        public FileHandleType HandleType { get; set; }

        public bool NoDelay { get; set; }
    }
}
