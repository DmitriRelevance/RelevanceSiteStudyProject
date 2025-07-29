using RelevanceSiteStudyProject.Core.DTOs;

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
    }
}
