namespace WebApi.Services
{
    public interface IKitService
    {
        Task<ServiceResponse<Kit>> Add(Kit addKit, string token);
    }
}