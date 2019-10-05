using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.TestHelpers.Fixtures
{
    public class ServerFixture : IDisposable
    {
        private readonly TimeoutFixture _timeout;
        private readonly ServicesFixture _services;
        private readonly AppFixture _app;
        private IServer _server;

        public ServerFixture(
            TimeoutFixture timeout,
            ServicesFixture services,
            AppFixture app)
        {
            _timeout = timeout;
            _services = services;
            _app = app;
        }

        public async Task StartAsync()
        {
            _server = _services.GetService<IServer>();
            await _server.StartAsync(_app, _timeout.Token);
        }

        public async Task StopAsync()
        {
            if (_server != null)
            {
                await _server.StopAsync(_timeout.Token);
                _server = null;
            }
        }

        public void Dispose()
        {
            StopAsync().GetAwaiter().GetResult();
        }
    }
}
