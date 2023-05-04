namespace WebApi.Services
{
    public interface IVariantService
    {
        Task<ServiceResponse<Variant>> Add(Variant addVariant, string token);
        Task<ServiceResponse<Variant>> Update(VariantUpdateDto variantInfo, string token);
        Task<ServiceResponse<string>> Delete(VariantDeleteDto variantInfo, string token);
    }
}