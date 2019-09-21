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
using System.Diagnostics;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Tests
{
    public class Http1Tests
    {
        [Fact]
        public async Task ServerCanStartAndStop()
        {
            using (var services = new ServicesFixture())
            using (var app = new AppFixture())
            using (var client = new HttpClient())
            using (var timeout = new CancellationTokenSource(Debugger.IsAttached ? TimeSpan.FromMinutes(5) : TimeSpan.FromSeconds(2.5)))
            {
                var server = services.GetService<IServer>();

                await server.StartAsync(app, timeout.Token);

                await server.StopAsync(timeout.Token);
            }
        }

        [Fact]
        public async Task GetMethodReturnsResponse()
        {
            using (var services = new ServicesFixture())
            using (var app = new AppFixture())
            using (var client = new HttpClient())
            using (var timeout = new CancellationTokenSource(Debugger.IsAttached ? TimeSpan.FromMinutes(5) : TimeSpan.FromSeconds(2.5)))
            {
                var server = services.GetService<IServer>();

                app.OnRequest = async context =>
                {
                    var request = context.Get<IHttpRequestFeature>();
                    var response = context.Get<IHttpResponseFeature>();

                    response.Headers["Content-Type"] = "text/plain";
                    response.Body.Write(Encoding.UTF8.GetBytes("Hello world!"));
                };

                await server.StartAsync(app, timeout.Token);

                var responseMessage = await client.GetAsync("http://localhost:5000/", timeout.Token);

                var responseBody = await responseMessage.Content.ReadAsStringAsync();

                Assert.Equal("Hello world!", responseBody);

                await server.StopAsync(timeout.Token);
            }
        }

        [Fact]
        public async Task MultipleGetMethodsEachExecute()
        {
            using (var services = new ServicesFixture())
            using (var app = new AppFixture())
            using (var client = new HttpClient())
            using (var timeout = new CancellationTokenSource(Debugger.IsAttached ? TimeSpan.FromMinutes(5) : TimeSpan.FromSeconds(2.5)))
            {
                var server = services.GetService<IServer>();

                await server.StartAsync(app, CancellationToken.None);

                var response1 = await client.GetAsync("http://localhost:5000/request1", timeout.Token);
                var response2 = await client.GetAsync("http://localhost:5000/request2", timeout.Token);
                var response3 = await client.GetAsync("http://localhost:5000/request3", timeout.Token);

                await server.StopAsync(timeout.Token);
            }
        }

        [Fact]
        public async Task ServerAcceptsPostBody()
        {
            using (var services = new ServicesFixture())
            using (var app = new AppFixture())
            using (var client = new HttpClient())
            using (var timeout = new CancellationTokenSource(Debugger.IsAttached ? TimeSpan.FromMinutes(5) : TimeSpan.FromSeconds(2.5)))
            {
                var server = services.GetService<IServer>();

                app.OnRequest = async context =>
                {
                    var request = context.Get<IHttpRequestFeature>();
                    var response = context.Get<IHttpResponseFeature>();

                    response.Headers["Content-Type"] = "text/plain";

                    await request.Body.CopyToAsync(response.Body);
                };

                await server.StartAsync(app, timeout.Token);

                var response1 = await client.PostAsync("http://localhost:5000/", new StringContent("Request Data One"), timeout.Token);
                var response2 = await client.PostAsync("http://localhost:5000/", new StringContent("Request Data Two"), timeout.Token);
                var response3 = await client.PostAsync("http://localhost:5000/", new StringContent("Request Data Three"), timeout.Token);

                await server.StopAsync(timeout.Token);
            }
        }

    }
}
