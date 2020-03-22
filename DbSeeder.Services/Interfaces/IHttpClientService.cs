using System.Net.Http;
using System.Threading.Tasks;

namespace DbSeeder.Services.Interfaces
{
    interface IHttpClientService
    {
        Task<HttpResponseMessage> SendMessageAsync(HttpMethod method, string fullUrl, string finalizedMessage);
    }
}
