namespace WebApi.Services
{
    public interface IKitService
    {
        Task<ServiceResponse<Kit>> Add(Kit addKit, string token);
        Task<ServiceResponse<Kit>> Update(KitUpdateDto kitInfo, string token);
        Task<ServiceResponse<List<ItemGetAllCombinations>>> GetAll();
    }
}