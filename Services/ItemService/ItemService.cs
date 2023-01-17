namespace WebApi.Services.ItemService
{
    public class ItemService : IItemService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        public ItemService(DataContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;

        }
        public ServiceResponse<Item[]> GetAll()
        {
            var response = new ServiceResponse<Item[]>();
            Item[] items = _context.Items.ToArray();
            response.Data = items;
            return response;
        }

        public async Task<ServiceResponse<Item>> GetById(int id)
        {
            var response = new ServiceResponse<Item>();
            Item item = _context.Items.ToList().Find(i => i.Id == id)!;
            if (item is not null)
                response.Data = item;
            else response.Success = false;
            return response;
        }
    }
}