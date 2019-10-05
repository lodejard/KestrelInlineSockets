// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using WheresLou.Server.Kestrel.Transport.InlineSockets.TestHelpers;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Fixtures;
using Xunit;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Tests
{
    public class Http1Tests
    {
        [Fact]
        public async Task ServerCanStartAndStop()
        {
            using var test = new TestContext();

            await test.Server.StartAsync();

            await test.Server.StopAsync();
        }

        [Fact]
        public async Task GetMethodReturnsResponse()
        {
            using var test = new TestContext();

            test.App.OnRequest = async message =>
            {
                var request = message.Get<IHttpRequestFeature>();
                var response = message.Get<IHttpResponseFeature>();

                response.Headers["Content-Type"] = "text/plain";

                var bytes = Encoding.UTF8.GetBytes("Hello world!");
                message.ResponseStream.Write(bytes, 0, bytes.Length);
            };

            await test.Server.StartAsync();

            var responseMessage = await test.Client.GetAsync("http://localhost:5000/", test.Timeout.Token);

            var responseBody = await responseMessage.Content.ReadAsStringAsync();

            Assert.Equal("Hello world!", responseBody);
        }

        [Fact]
        public async Task MultipleGetMethodsEachExecute()
        {
            using var test = new TestContext();

            await test.Server.StartAsync();

            var response1 = await test.Client.GetAsync("http://localhost:5000/request1", test.Timeout.Token);
            var response2 = await test.Client.GetAsync("http://localhost:5000/request2", test.Timeout.Token);
            var response3 = await test.Client.GetAsync("http://localhost:5000/request3", test.Timeout.Token);
        }

        [Fact]
        public virtual async Task ServerAcceptsPostBody()
        {
            using var test = new TestContext();

            test.App.OnRequest = async message =>
            {
                var request = message.Get<IHttpRequestFeature>();
                var response = message.Get<IHttpResponseFeature>();

                response.Headers["Content-Type"] = "text/plain";

                using (var reader = new StreamReader(request.Body))
                {
                    var text = reader.ReadToEnd();
                    using (var writer = new StreamWriter(message.ResponseStream))
                    {
                        writer.Write(text);
                    }
                }
            };

            await test.Server.StartAsync();

            var response1 = await test.Client.PostAsync("http://localhost:5000/", new StringContent("Request Data One"), test.Timeout.Token);
            var response2 = await test.Client.PostAsync("http://localhost:5000/", new StringContent("Request Data Two"), test.Timeout.Token);
            var response3 = await test.Client.PostAsync("http://localhost:5000/", new StringContent("Request Data Three"), test.Timeout.Token);

        }
    }
}
