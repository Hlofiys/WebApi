namespace WebApi.Services
{
    public interface IVariantService
    {
        Task<ServiceResponse<Variant>> Add(Variant addVariant, string token);
    }
}