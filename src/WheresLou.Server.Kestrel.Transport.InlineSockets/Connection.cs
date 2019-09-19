using System.Net;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class Connection : TransportConnection, IHttpConnectionFeature
    {
        private readonly ILogger<Connection> _logger;
        private readonly ConnectionContext _context;
        private string _connectionId;

        public Connection(
            ILogger<Connection> logger,
            ConnectionContext context)
        {
            _logger = logger;
            _context = context;
        }

        public override string ConnectionId
        {
            get => _connectionId;
            set => _connectionId = value;
        }

        string IHttpConnectionFeature.ConnectionId
        {
            get => _connectionId;
            set => _connectionId = value;
        }

        IPAddress IHttpConnectionFeature.RemoteIpAddress
        {
            get => ((IPEndPoint)_context.Socket.RemoteEndPoint).Address;
            set => throw new System.NotImplementedException();
        }

        IPAddress IHttpConnectionFeature.LocalIpAddress
        {
            get => ((IPEndPoint)_context.Socket.LocalEndPoint).Address;
            set => throw new System.NotImplementedException();
        }

        int IHttpConnectionFeature.RemotePort
        {
            get => ((IPEndPoint)_context.Socket.RemoteEndPoint).Port;
            set => throw new System.NotImplementedException();
        }

        int IHttpConnectionFeature.LocalPort
        {
            get => ((IPEndPoint)_context.Socket.LocalEndPoint).Port;
            set => throw new System.NotImplementedException();
        }
    }
}
