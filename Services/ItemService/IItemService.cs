namespace WebApi.Services
{
    public interface IItemService
    {
        ServiceResponse<Item[]> GetAll();
        Task<ServiceResponse<ItemById>> GetById(int id);
        Task<ServiceResponse<Item>> Add(Item addItem, string token);
    }
}