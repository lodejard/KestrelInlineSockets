using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Stubs
{
    public class TestConnectionDispatcher : IConnectionDispatcher
    {
        public List<TransportConnection> Connections { get; } = new List<TransportConnection>();

        public Task OnConnection(TransportConnection connection)
        {
            Connections.Add(connection);
            return Task.CompletedTask;
        }
    }
}
