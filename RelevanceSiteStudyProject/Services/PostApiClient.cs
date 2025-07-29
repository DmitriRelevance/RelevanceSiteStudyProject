using RelevanceSiteStudyProject.Core.DTOs;
using System.Text;
using System.Text.Json;

namespace RelevanceSiteStudyProject.Services
{
    public class PostApiClient
    {
        private readonly HttpClient _httpClient;
        public PostApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IList<PostDto>> GetPostsAsync()
        {
            var response = await _httpClient.GetAsync("posts");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IList<PostDto>>() ?? new List<PostDto>();
        }

        public async Task<PostDto?> AddPostAsync(PostCreateDto postToCreate)
        {
            var content = new StringContent(JsonSerializer.Serialize(postToCreate), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"posts", content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PostDto>();
            }
            return null;
        }
    }
}
