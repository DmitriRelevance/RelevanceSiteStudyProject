using Microsoft.JSInterop;
using RelevanceSiteStudyProject.Core.Interfaces;

namespace RelevanceSiteStudy.Services.Services
{
    public class LocalStorageTokenProvider : ITokenProvider
    {
        private readonly IJSRuntime _js;

        private const string TokenKey = "authToken";

        public LocalStorageTokenProvider(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _js.InvokeAsync<string>("localStorage.getItem", TokenKey);
        }

        public async Task SetTokenAsync(string token)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        }

        public async Task ClearTokenAsync()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        }
    }

}
