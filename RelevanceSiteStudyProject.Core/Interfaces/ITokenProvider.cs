namespace RelevanceSiteStudyProject.Core.Interfaces
{
    public interface ITokenProvider
    {
        Task<string?> GetTokenAsync();
        Task SetTokenAsync(string token);
        Task ClearTokenAsync();
    }
}
