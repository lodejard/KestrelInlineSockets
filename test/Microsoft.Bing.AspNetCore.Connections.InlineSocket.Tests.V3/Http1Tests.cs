// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.TestHelpers;
using Xunit;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Tests
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

            var responseMessage = await test.Client.GetAsync("/");

            var responseBody = await responseMessage.Content.ReadAsStringAsync();

            Assert.Equal("Hello world!", responseBody);
        }

        [Fact]
        public async Task MultipleGetMethodsEachExecute()
        {
            using var test = new TestContext();

            await test.Server.StartAsync();

            var response1 = await test.Client.GetAsync("/request1");
            var response2 = await test.Client.GetAsync("/request2");
            var response3 = await test.Client.GetAsync("/request3");
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

            var response1 = await test.Client.PostStringAsync("/", new StringContent("Request Data One"));
            var response2 = await test.Client.PostStringAsync("/", new StringContent("Request Data Two"));
            var response3 = await test.Client.PostStringAsync("/", new StringContent("Request Data Three"));
        }

        [Fact]
        public virtual async Task VeryLargeRequestAndResponseBody()
        {
            using var test = new TestContext();

            test.App.OnRequest = async message =>
            {
                var request = message.Get<IHttpRequestFeature>();
                var response = message.Get<IHttpResponseFeature>();

                response.Headers["Content-Type"] = "text/plain";

                var memory = new MemoryStream();
                request.Body.CopyTo(memory);
                message.ResponseStream.Write(memory.ToArray());
            };

            await test.Server.StartAsync();

            var bytes1 = new byte[1 << 10]; // 1kb
            var bytes2 = new byte[1 << 15]; // 32kb
            var bytes3 = new byte[1 << 20]; // 1mb

            var random = new Random();
            random.NextBytes(bytes1);
            random.NextBytes(bytes2);
            random.NextBytes(bytes3);

            var response1 = await test.Client.PostBytesAsync("/", new ByteArrayContent(bytes1));
            var response2 = await test.Client.PostBytesAsync("/", new ByteArrayContent(bytes2));
            var response3 = await test.Client.PostBytesAsync("/", new ByteArrayContent(bytes3));

            await test.Server.StopAsync();

            Assert.All(response1.content.Zip(bytes1, (a, b) => (a, b)), pair => Assert.Equal(pair.a, pair.b));
            Assert.All(response2.content.Zip(bytes2, (a, b) => (a, b)), pair => Assert.Equal(pair.a, pair.b));
            Assert.All(response3.content.Zip(bytes3, (a, b) => (a, b)), pair => Assert.Equal(pair.a, pair.b));
        }

        [Fact]
        public virtual async Task ConnectionKeepAliveByDefault()
        {
            using var test = new TestContext();

            var connectionIds = new List<string>();

            test.App.OnRequest = async message =>
            {
                var request = message.Get<IHttpRequestFeature>();
                var response = message.Get<IHttpResponseFeature>();
                var connection = message.Get<IHttpConnectionFeature>();

                response.Headers["Content-Type"] = "text/plain";

                connectionIds.Add(connection.ConnectionId);

                var bytes = Encoding.UTF8.GetBytes("Hello world!");
                message.ResponseStream.Write(bytes, 0, bytes.Length);
            };

            await test.Server.StartAsync();

            var response1 = await test.Client.GetStringAsync("/");
            var response2 = await test.Client.GetStringAsync("/");
            var response3 = await test.Client.GetStringAsync("/");

            Assert.Single(connectionIds.Distinct());

            Assert.Single(from log in test.Logging.LogItems where log.EventId.Name == "SocketAccepted" select log);
        }

        [Fact]
        public virtual async Task ConnectionCloseCanBeProvided()
        {
            using var test = new TestContext();

            var connectionIds = new List<string>();

            test.App.OnRequest = async message =>
            {
                var request = message.Get<IHttpRequestFeature>();
                var response = message.Get<IHttpResponseFeature>();
                var connection = message.Get<IHttpConnectionFeature>();

                response.Headers["Content-Type"] = "text/plain";
                response.Headers["Connection"] = "close";

                connectionIds.Add(connection.ConnectionId);

                var bytes = Encoding.UTF8.GetBytes("Hello world!");
                message.ResponseStream.Write(bytes, 0, bytes.Length);
            };

            await test.Server.StartAsync();

            var response1 = await test.Client.GetStringAsync("/");
            var response2 = await test.Client.GetStringAsync("/");
            var response3 = await test.Client.GetStringAsync("/");

            Assert.Equal(3, connectionIds.Distinct().Count());

            Assert.Equal(3, (from log in test.Logging.LogItems where log.EventId.Name == "SocketAccepted" select log).Count());
        }
    }
}
