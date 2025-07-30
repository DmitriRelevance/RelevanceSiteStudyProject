using RelevanceSiteStudyProject.Core.DTOs;
using RelevanceSiteStudyProject.Core.Interfaces;
using System.Text;
using System.Text.Json;

namespace RelevanceSiteStudyProject.Services
{
    public class PostApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenProvider _tokenProvider;
        public PostApiClient(HttpClient httpClient, ITokenProvider tokenProvider)
        {
            _httpClient = httpClient;
            _tokenProvider = tokenProvider;
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

        public async Task UpdatePostAsync(PostDto post)
        {
            await SetAuthorizationHeaderAsync();

            string serializedJson = JsonSerializer.Serialize(post);

            var content = new StringContent(serializedJson, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"posts/{post.Id}", content);
            response.EnsureSuccessStatusCode();
        }


        public async Task DeletePostAsync(int postId)
        {
            await SetAuthorizationHeaderAsync();

            var response = await _httpClient.DeleteAsync($"posts/{postId}");
            response.EnsureSuccessStatusCode();
        }


        private async Task SetAuthorizationHeaderAsync()
        {
            var token = await _tokenProvider.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("No valid token found.");
            }
            else
            {
                token = token.Trim('"'); // Fix it
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
