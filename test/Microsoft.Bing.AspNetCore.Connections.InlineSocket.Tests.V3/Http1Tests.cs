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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Tests
{
    public class Http1Tests : IDisposable
    {
        public Http1Tests(ITestOutputHelper output)
        {
            Output = output;
            Test = new TestContext();
        }

        public ITestOutputHelper Output { get; }
        public TestContext Test { get; }

        void IDisposable.Dispose()
        {
            Test.Logging.WriteTo(Output.WriteLine);
            Test.Dispose();
        }


        [Theory]
        [InlineData("http")]
        [InlineData("https")]
        public async Task ServerCanStartAndStop(string scheme)
        {
            Test.EndPoint.Scheme = scheme;

            await Test.Server.StartAsync();

            await Test.Server.StopAsync();
        }

        [Fact]
        public async Task GetMethodReturnsResponse()
        {
            Test.App.OnRequest = async message =>
            {
                var request = message.Get<IHttpRequestFeature>();
                var response = message.Get<IHttpResponseFeature>();

                response.Headers["Content-Type"] = "text/plain";

                var bytes = Encoding.UTF8.GetBytes("Hello world!");
                message.ResponseStream.Write(bytes, 0, bytes.Length);
            };

            await Test.Server.StartAsync();

            var responseMessage = await Test.Client.GetAsync("/");

            var responseBody = await responseMessage.Content.ReadAsStringAsync();

            Assert.Equal("Hello world!", responseBody);
        }

        [Fact]
        public async Task MultipleGetMethodsEachExecute()
        {
            await Test.Server.StartAsync();

            var response1 = await Test.Client.GetAsync("/request1");
            var response2 = await Test.Client.GetAsync("/request2");
            var response3 = await Test.Client.GetAsync("/request3");
        }

        [Fact]
        public virtual async Task ServerAcceptsPostBody()
        {
            Test.App.OnRequest = async message =>
            {
                var request = message.Get<IHttpRequestFeature>();
                var response = message.Get<IHttpResponseFeature>();

                response.Headers["Content-Type"] = "text/plain";

                using var reader = new StreamReader(request.Body);
                var text = reader.ReadToEnd();
                
                using var writer = new StreamWriter(message.ResponseStream);
                writer.Write(text);
            };

            await Test.Server.StartAsync();

            var response1 = await Test.Client.PostStringAsync("/", new StringContent("Request Data One"));
            var response2 = await Test.Client.PostStringAsync("/", new StringContent("Request Data Two"));
            var response3 = await Test.Client.PostStringAsync("/", new StringContent("Request Data Three"));
        }

        [Fact]
        public virtual async Task VeryLargeRequestAndResponseBody()
        {
            Test.App.OnRequest = async message =>
            {
                var request = message.Get<IHttpRequestFeature>();
                var response = message.Get<IHttpResponseFeature>();

                response.Headers["Content-Type"] = "text/plain";

                var memory = new MemoryStream();
                request.Body.CopyTo(memory);
                message.ResponseStream.Write(memory.ToArray());
            };

            await Test.Server.StartAsync();

            var bytes1 = new byte[1 << 10]; // 1kb
            var bytes2 = new byte[1 << 15]; // 32kb
            var bytes3 = new byte[1 << 20]; // 1mb

            var random = new Random();
            random.NextBytes(bytes1);
            random.NextBytes(bytes2);
            random.NextBytes(bytes3);

            var response1 = await Test.Client.PostBytesAsync("/", new ByteArrayContent(bytes1));
            var response2 = await Test.Client.PostBytesAsync("/", new ByteArrayContent(bytes2));
            var response3 = await Test.Client.PostBytesAsync("/", new ByteArrayContent(bytes3));

            Assert.Equal(bytes1.Length, response1.content.Length);
            Assert.Equal(bytes2.Length, response2.content.Length);
            Assert.Equal(bytes3.Length, response3.content.Length);

            await Test.Server.StopAsync();

            Assert.All(response1.content.Zip(bytes1, (a, b) => (a, b)), pair => Assert.Equal(pair.a, pair.b));
            Assert.All(response2.content.Zip(bytes2, (a, b) => (a, b)), pair => Assert.Equal(pair.a, pair.b));
            Assert.All(response3.content.Zip(bytes3, (a, b) => (a, b)), pair => Assert.Equal(pair.a, pair.b));
        }

        [Theory]
        [InlineData("http")]
        [InlineData("https")]
        public virtual async Task ConnectionKeepAliveByDefault(string scheme)
        {
            Test.EndPoint.Scheme = scheme;

            var connectionIds = new List<string>();

            Test.App.OnRequest = async message =>
            {
                var request = message.Get<IHttpRequestFeature>();
                var response = message.Get<IHttpResponseFeature>();
                var connection = message.Get<IHttpConnectionFeature>();

                response.Headers["Content-Type"] = "text/plain";

                connectionIds.Add(connection.ConnectionId);

                var bytes = Encoding.UTF8.GetBytes("Hello world!");
                message.ResponseStream.Write(bytes, 0, bytes.Length);
            };

            await Test.Server.StartAsync();

            var response1 = await Test.Client.GetStringAsync("/");
            var response2 = await Test.Client.GetStringAsync("/");
            var response3 = await Test.Client.GetStringAsync("/");

            Assert.Single(connectionIds.Distinct());

            Assert.Single(from log in Test.Logging.LogItems where log.EventId.Name == "SocketAccepted" select log);
        }

        [Theory]
        [InlineData("http")]
#if NETCOREAPP3_0
        [InlineData("https")]
#else
        // connection close on 2.2 over TLS isn't shutting down properly
         [InlineData("https")]
#endif
        public virtual async Task ConnectionCloseCanBeProvided(string scheme)
        {
            Test.EndPoint.Scheme = scheme;

            var connectionIds = new List<string>();

            Test.App.OnRequest = async message =>
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

            await Test.Server.StartAsync();

            var response1 = await Test.Client.GetStringAsync("/");
            var response2 = await Test.Client.GetStringAsync("/");
            var response3 = await Test.Client.GetStringAsync("/");

            Assert.Equal(3, connectionIds.Distinct().Count());

            Assert.Equal(3, (from log in Test.Logging.LogItems where log.EventId.Name == "SocketAccepted" select log).Count());
        }


        [Theory]
        [InlineData("http")]
        [InlineData("https")]
        public virtual async Task RequestCanBeAborted(string scheme)
        {
            Test.EndPoint.Scheme = scheme;

            var connectionIds = new List<string>();

            Test.App.OnRequest = async message =>
            {
                var httpContext = new DefaultHttpContext(message.Features);
                httpContext.Abort();
            };

            await Test.Server.StartAsync();

            var error1 = await Assert.ThrowsAsync<HttpRequestException>(async () => await Test.Client.GetStringAsync("/request1"));
            var error2 = await Assert.ThrowsAsync<HttpRequestException>(async () => await Test.Client.GetStringAsync("/request2"));
            var error3 = await Assert.ThrowsAsync<HttpRequestException>(async () => await Test.Client.GetStringAsync("/request3"));
        }

        public async Task<(Exception outer, T inner)> ThrowsInnerAsync<T>(Func<Task> code)
        {
            try
            {
                await code();
            }
            catch (Exception outer) when (FindInner(outer) != default)
            {
                return (outer, FindInner(outer));
            }
            throw new Exception($"{typeof(T).FullName} was not thrown");

            static T FindInner(Exception ex)
            {
                for (var scan = ex; scan != null; scan = scan.InnerException)
                {
                    if (scan is T t)
                    {
                        return t;
                    }
                }
                return default;
            }
        }

    }
}
