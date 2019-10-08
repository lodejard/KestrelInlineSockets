using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Tests.Fixtures;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.TestHelpers.Fixtures
{
    public class ClientFixture
    {
        private readonly EndPointFixture _endPoint;
        private readonly HttpClient _httpClient;
        private readonly TimeoutFixture _timeout;

        public ClientFixture(EndPointFixture endPoint, HttpClient httpClient, TimeoutFixture timeout)
        {
            _endPoint = endPoint;
            _httpClient = httpClient;
            _timeout = timeout;
        }

        public async Task<HttpResponseMessage> GetAsync(string path)
        {
            return await _httpClient.GetAsync(_endPoint.Address + path, HttpCompletionOption.ResponseHeadersRead, _timeout.Token);
        }

        public async Task<(HttpResponseMessage response, string content)> GetStringAsync(string path)
        {
            var response = await GetAsync(path);
            return (response, await response.Content.ReadAsStringAsync());
        }

        public async Task<HttpResponseMessage> PostAsync(string path, HttpContent content)
        {
            return await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, _endPoint.Address + path) { Content = content }, HttpCompletionOption.ResponseHeadersRead, _timeout.Token);
        }

        public async Task<(HttpResponseMessage response, string content)> PostStringAsync(string path, HttpContent content)
        {
            var response = await GetAsync(path);
            return (response, await response.Content.ReadAsStringAsync());
        }

        public async Task<(HttpResponseMessage response, byte[] content)> PostBytesAsync(string path, HttpContent content)
        {
            var response = await GetAsync(path);
            return (response, await response.Content.ReadAsByteArrayAsync());
        }
    }
}
