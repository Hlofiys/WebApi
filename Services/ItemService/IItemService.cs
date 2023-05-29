namespace WebApi.Services
{
    public interface IItemService
    {
        ServiceResponse<List<Item>> GetAll();
        Task<ServiceResponse<ItemById>> GetById(int id);
        Task<ServiceResponse<Item>> Add(Item addItem, string token);
        Task<ServiceResponse<Item>> Update(ItemUpdateDto itemInfo, string token);
        Task<ServiceResponse<List<ItemGetAllCombinations>>> GetAllCombinatios (int id);
    }
}