using Microsoft.AspNetCore.Http.HttpResults;
using static System.Net.Mime.MediaTypeNames;

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
                List<string> images = new List<string>(){ "http://129.159.242.47:8081/media/get/podstavki?name=1_main.png", "http://129.159.242.47:8081/media/get/podstavki?name=1_second.png" };
                List<string> images1 = new List<string>() { "http://129.159.242.47:8081/media/get/podstavki?name=2_main.png", "http://129.159.242.47:8081/media/get/podstavki?name=2_second.png" };
                List<string> images2 = new List<string>() { "http://129.159.242.47:8081/media/get/podstavki?name=3_main.png", "http://129.159.242.47:8081/media/get/podstavki?name=3_second.png" };
                List<string> images3 = new List<string>() { "http://129.159.242.47:8081/media/get/podstavki?name=4_main.png", "http://129.159.242.47:8081/media/get/podstavki?name=4_second.png", "http://129.159.242.47:8081/media/get/podstavki?name=4_third.png" };
                var item = new Item
                {
                    Id = index,
                    Name = "Подставка",
                    Description = "",
                    Price = 10,
                    Icon = images,
                    TypeId = types.Find(i => i.Id == 1)?.Id.ToString()!,
                    Video = "http://129.159.242.47:8081/media/get/podstavki?name=1_video.mp4",
                };
                Variant variant1 = new Variant
                {
                    VariantId = 1,
                    Name = "Подсвечник",
                    Description = "",
                    Price = 15,
                    Icon = images1,
                    Video = "",
                    ItemId = index,
                };
                Variant variant2 = new Variant
                {
                    VariantId = 2,
                    Name = "Шкатулка",
                    Description = "",
                    Price = 20,
                    Icon = images3,
                    Video = "",
                    ItemId = index,
                };
                Kit kit = new Kit
                {
                    KitId = 1,
                    Name = "Подсвечник + шкатулка",
                    Description = "",
                    Price = 25,
                    Variants =  new List<int>() { 1, 2},
                    Icon = images2,
                    Video = "",
                    ItemId = index,
                };
                _context.Items.Add(item);
                _context.SaveChanges();
                _context.Variants.Add(variant1);
                _context.Variants.Add(variant2);
                _context.Kits.Add(kit);
                _context.SaveChanges();
            }
            index++;
            if (!ItemExists(index))
            {
                List<string> images1 = new() { "http://129.159.242.47:8081/media/get/lilu?name=1_main.png", "http://129.159.242.47:8081/media/get/lilu?name=1_second.png", "http://129.159.242.47:8081/media/get/lilu?name=1_third.png" };
                List<string> images2 = new() { "http://129.159.242.47:8081/media/get/lilu?name=2_main.png", "http://129.159.242.47:8081/media/get/lilu?name=2_second.png", "http://129.159.242.47:8081/media/get/lilu?name=2_third.png" };
                List<string> images3 = new() { "http://129.159.242.47:8081/media/get/lilu?name=3_main.png", "http://129.159.242.47:8081/media/get/lilu?name=3_second.png", "http://129.159.242.47:8081/media/get/lilu?name=3_third.png" };
                List<string> images4 = new() { "http://129.159.242.47:8081/media/get/lilu?name=4_main.png", "http://129.159.242.47:8081/media/get/lilu?name=4_second.png", "http://129.159.242.47:8081/media/get/lilu?name=4_third.png" };
                var item = new Item
                {
                    Id = index,
                    Name = "Лилу",
                    Description = "",
                    Price = 55,
                    Icon = images1,
                    TypeId = types.Find(i => i.Id == 3)?.Id.ToString()!,
                    Video = "http://129.159.242.47:8081/media/get/lilu?name=1_video.mp4",
                };
                Variant variant1 = new Variant
                {
                    VariantId = 1,
                    Name = "Стрелки",
                    Description = "",
                    Price = 58,
                    Icon = images1,
                    Video = "",
                    ItemId = index,
                };
                Variant variant2 = new Variant
                {
                    VariantId = 2,
                    Name = "Шипы",
                    Description = "",
                    Price = 60,
                    Icon = images3,
                    Video = "",
                    ItemId = index,
                };
                Variant variant3 = new Variant
                {
                    VariantId = 3,
                    Name = "Патчи",
                    Description = "",
                    Price = 60,
                    Icon = images3,
                    Video = "",
                    ItemId = index,
                };
                Kit kit1 = new Kit
                {
                    KitId = 1,
                    Name = "Стрелки + Шипы",
                    Description = "",
                    Price = 60,
                    Variants = new List<int>() { 1, 2 },
                    Icon = images3,
                    Video = "",
                    ItemId = index,
                };
                Kit kit2 = new Kit
                {
                    KitId = 2,
                    Name = "Патчи + Шипы",
                    Description = "",
                    Price = 60,
                    Variants = new List<int>() { 2, 3 },
                    Icon = new List<string> { "http://129.159.242.47:8081/media/get/lilu?name=4_third.png", "http://129.159.242.47:8081/media/get/lilu?name=3_main.png", "http://129.159.242.47:8081/media/get/lilu?name=4_main.png" },
                    Video = "",
                    ItemId = index,
                };
                Kit kit3 = new Kit
                {
                    KitId = 3,
                    Name = "Стрелки + Патчи",
                    Description = "",
                    Price = 60,
                    Variants = new List<int>() { 1, 3 },
                    Icon = new List<string> { "http://129.159.242.47:8081/media/get/lilu?name=4_third.png", "http://129.159.242.47:8081/media/get/lilu?name=4_second.png" },
                    Video = "",
                    ItemId = index,
                };
                Kit kit4 = new Kit
                {
                    KitId = 4,
                    Name = "Стрелки + Патчи + шипы",
                    Description = "",
                    Price = 60,
                    Variants = new List<int>() { 1, 2, 3 },
                    Icon = new List<string> { "http://129.159.242.47:8081/media/get/lilu?name=4_third.png", "http://129.159.242.47:8081/media/get/lilu?name=4_second.png", "http://129.159.242.47:8081/media/get/lilu?name=3_third.png" },
                    Video = "",
                    ItemId = index,
                };
                _context.Items.Add(item);
                _context.SaveChanges();
                _context.Variants.Add(variant1);
                _context.Variants.Add(variant2);
                _context.Variants.Add(variant3);
                _context.Kits.Add(kit1);
                _context.Kits.Add(kit2);
                _context.Kits.Add(kit3);
                _context.Kits.Add(kit4);
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
                    Icon = "http://129.159.242.47:8081/media/get/podstavki?name=1_main.png"
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
                    Icon = "http://129.159.242.47:8081/media/get/lilu?name=2_second.png"
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