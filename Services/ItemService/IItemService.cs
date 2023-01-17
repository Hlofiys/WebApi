namespace WebApi.Services.ItemService
{
    public interface IItemService
    {
        ServiceResponse<Item[]> GetAll();
        Task<ServiceResponse<Item>> GetByiD(int id);
    }
}