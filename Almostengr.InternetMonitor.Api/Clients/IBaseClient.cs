using System.Net.Http;
using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.DataTransfer;

namespace Almostengr.InternetMonitor.Api.Clients
{
    public interface IBaseClient
    {
        Task<T> HttpGetAsync<T>(HttpClient httpClient, string route) where T : BaseDto;
        Task<T> HttpPostAsync<T>(HttpClient httpClient, string route, string body) where T : BaseDto;
    }
}