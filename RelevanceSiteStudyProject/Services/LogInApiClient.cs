using RelevanceSiteStudyProject.Core.DTOs;
using System.Text;
using System.Text.Json;

namespace RelevanceSiteStudyProject.Services
{
    public class LogInApiClient
    {
        private readonly HttpClient _httpClient;
        public LogInApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> LogInAsync(LoginDto login)
        {
            var content = new StringContent(JsonSerializer.Serialize(login), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("login", content);
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                return token;
            }
            return null;
        }
    }
}
