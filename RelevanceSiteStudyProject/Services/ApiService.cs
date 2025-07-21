using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RelevanceSiteStudyProject.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T?> GetDataAsync<T>(string url)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(url);

                if (string.IsNullOrEmpty(response))
                {
                    throw new Exception("No data received from the API.");
                }

                return JsonConvert.DeserializeObject<T>(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");

                return default(T);
            }
        }
    }

}
