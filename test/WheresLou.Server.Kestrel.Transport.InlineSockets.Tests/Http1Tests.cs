using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Fixtures;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Features;
using System.Threading;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Tests
{
    public class Http1Tests
    {
        [Fact]
        public async Task KestrelAcceptsRequests()
        {
            using (var services = new ServicesFixture())
            using (var app = new AppFixture())
            using (var client = new HttpClient())
            using (var timeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(250)))
            {
                var server = services.GetService<IServer>();
                var options = services.GetService<IOptions<KestrelServerOptions>>();

                await server.StartAsync(app, CancellationToken.None);

                var response = await client.GetAsync("http://localhost:5000/", timeout.Token);

            }
        }

        
    }
}
