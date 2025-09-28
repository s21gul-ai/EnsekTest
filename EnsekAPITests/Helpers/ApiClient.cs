using Microsoft.Playwright;
using System.Threading.Tasks;

namespace EnsekAPITests.Helpers
{
    public class ApiClient
    {
        private readonly IAPIRequestContext _api;

        public ApiClient(IAPIRequestContext api)
        {
            _api = api;
        }

        public async Task<IAPIResponse> GetAsync(string endpoint)
        {
            return await _api.GetAsync(endpoint);
        }

        public async Task<IAPIResponse> PostAsync(string endpoint, object body)
        {
            return await _api.PostAsync(endpoint, new APIRequestContextOptions
            {
                DataObject = body
            });
        }
    }
}
