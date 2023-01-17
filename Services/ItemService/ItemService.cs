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
            Console.WriteLine(items);
            response.Data = items;
            return response;
        }

        public Task<ServiceResponse<Item>> GetByiD(int id)
        {
            throw new NotImplementedException();
        }
    }
}