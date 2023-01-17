using Microsoft.AspNetCore.Http.HttpResults;

namespace WebApi
{
    public class StartUp
    {
        private readonly DataContext _context;
        public StartUp(DataContext context)
        {
            _context = context;

        }
        public async Task AddItems()
        {
            var types = _context.Types.ToList();
            int index = 1;
            if (!ItemExists(index))
            {
                List<string> images = new List<string>(){ "https://api.hlofiys.tk/media/get/podstavki?name=1_main.png", "https://api.hlofiys.tk/media/get/podstavki?name=1_second.png" };
                List<string> images1 = new List<string>() { "https://api.hlofiys.tk/media/get/podstavki?name=2_main.png", "https://api.hlofiys.tk/media/get/podstavki?name=2_second.png" };
                List<string> images2 = new List<string>() { "https://api.hlofiys.tk/media/get/podstavki?name=3_main.png", "https://api.hlofiys.tk/media/get/podstavki?name=3_second.png" };
                List<string> images3 = new List<string>() { "https://api.hlofiys.tk/media/get/podstavki?name=4_main.png", "https://api.hlofiys.tk/media/get/podstavki?name=4_second.png", "https://api.hlofiys.tk/media/get/podstavki?name=4_third.png" };
                var item = new Item
                {
                    Id = index,
                    Name = "Подставка",
                    Description = "",
                    Price = 10,
                    Icon = images,
                    TypeId = types.Find(i => i.Id == 1)?.Id.ToString()!,
                    Video = "https://api.hlofiys.tk/media/get/podstavki?name=1_video.mp4",
                };
                Variant variant1 = new Variant
                {
                    Id = 1,
                    Name = "Подсвечник",
                    Description = "",
                    Price = 15,
                    Icon = images1,
                    Video = "",
                    ItemId = index,
                };
                Variant variant2 = new Variant
                {
                    Id = 2,
                    Name = "Шкатулка",
                    Description = "",
                    Price = 20,
                    Icon = images3,
                    Video = "",
                    ItemId = index,
                };
                Variant variant3 = new Variant
                {
                    Id = 3,
                    Name = "Подсвечник + шкатулка",
                    Description = "",
                    Price = 25,
                    Icon = images2,
                    Video = "",
                    ItemId = index,
                };
                _context.Items.Add(item);
                _context.SaveChanges();
                _context.Variants.Add(variant1);
                _context.Variants.Add(variant2);
                _context.Variants.Add(variant3);
                _context.SaveChanges();
            }
            index++;
        }
        private bool ItemExists(int id)
        {
            var items = _context.Items.ToList();
            if (items.Find(i => i.Id == id) is not null)
            {
                return true;
            }
            return false;
        }

        public async Task AddTypes()
        {
            int index = 1;
            if (!TypeExists(index))
            {
                Models.Type type = new Models.Type
                {
                    Id = index,
                    Name = "Подставка",
                    Description = "",
                    Price = 10,
                    Icon = "https://api.hlofiys.tk/media/get/podstavki?name=1_main.png"
                };
                _context.Types.Add(type);
            }
            index++;
            if (!TypeExists(index))
            {
                Models.Type type = new Models.Type
                {
                    Id = index,
                    Name = "Шкатулка",
                    Description = "",
                    Price = 15,
                    Icon = ""
                };
                _context.Types.Add(type);
            }
            index++;
            if (!TypeExists(index))
            {
                Models.Type type = new Models.Type
                {
                    Id = index,
                    Name = "Лилу",
                    Description = "",
                    Price = 55,
                    Icon = "https://api.hlofiys.tk/media/get/lilu?name=2_second.png"
                };
                _context.Types.Add(type);
            }
            index++;
            if (!TypeExists(index))
            {
                Models.Type type = new Models.Type
                {
                    Id = index,
                    Name = "Берни",
                    Description = "",
                    Price = 50,
                    Icon = ""
                };
                _context.Types.Add(type);
            }
            _context.SaveChanges();
        }

        private bool TypeExists(int id)
        {
            var types = _context.Types.ToList();
            if (types.Find(i => i.Id == id) is not null)
            {
                return true;
            }
            return false;
        }
    }
}