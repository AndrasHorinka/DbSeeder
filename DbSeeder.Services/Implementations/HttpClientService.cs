using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DbSeeder.Services.Implementations
{
    public static class HttpClientService
    {
        static readonly HttpClient client = new HttpClient();

        public static async Task<HttpResponseMessage> SendRequestAsync(string url, byte[] content, HttpMethod method)
        {
            client.Timeout = TimeSpan.FromSeconds(30.0);

            try
            {
                HttpRequestMessage message = new HttpRequestMessage
                {
                    Method = method,
                    RequestUri = new Uri(url),
                    Content = new ByteArrayContent(content)
                };
                var response = await client.SendAsync(message);
                return response;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }
    }
}
