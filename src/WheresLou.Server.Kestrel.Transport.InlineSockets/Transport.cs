using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class Transport : ITransport
    {
        private readonly ILogger<Transport> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IEndPointInformation _endPointInformation;
        private readonly IConnectionDispatcher _dispatcher;

        public Transport(ILogger<Transport> logger, IConnectionFactory connectionFactory, IEndPointInformation endPointInformation, IConnectionDispatcher dispatcher)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
            _endPointInformation = endPointInformation;
            _dispatcher = dispatcher;
        }

        public Task BindAsync()
        {
            throw new NotImplementedException();
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }

        public Task UnbindAsync()
        {
            throw new NotImplementedException();
        }
    }
}
