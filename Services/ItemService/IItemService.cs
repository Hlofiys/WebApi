namespace WebApi.Services.ItemService
{
    public interface IItemService
    {
        ServiceResponse<Item[]> GetAll();
        Task<ServiceResponse<ItemById>> GetById(int id);
    }
}