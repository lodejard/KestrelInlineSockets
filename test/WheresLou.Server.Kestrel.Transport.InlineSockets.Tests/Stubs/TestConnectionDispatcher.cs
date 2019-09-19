using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Stubs
{
    internal class TestConnectionDispatcher : IConnectionDispatcher
    {
        public List<TransportConnection> Connections = new List<TransportConnection>();

        public Task OnConnection(TransportConnection connection)
        {
            Connections.Add(connection);
            return Task.CompletedTask;
        }
    }
}
