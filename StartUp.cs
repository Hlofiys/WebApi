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
            int index = 1;
            if (!ItemExists(index))
            {
                List<string> images = new List<string>() { "https://api.native-flora.tk/media/get/podstavki?name=1_main.png", "https://api.native-flora.tk/media/get/podstavki?name=1_second.png" };
                List<string> images1 = new List<string>() { "https://api.native-flora.tk/media/get/podstavki?name=2_main.png", "https://api.native-flora.tk/media/get/podstavki?name=2_second.png" };
                List<string> images2 = new List<string>() { "https://api.native-flora.tk/media/get/podstavki?name=3_main.png", "https://api.native-flora.tk/media/get/podstavki?name=3_second.png" };
                List<string> images3 = new List<string>() { "https://api.native-flora.tk/media/get/podstavki?name=4_main.png", "https://api.native-flora.tk/media/get/podstavki?name=4_second.png", "https://api.native-flora.tk/media/get/podstavki?name=4_third.png" };
                var item = new Item
                {
                    Id = index,
                    Name = "Подставка",
                    Description = new List<string> { "Подставка – органайзер выполнена из гипсобетона, он прочнее и тяжелее гипса. Покрыта лаком. Изделие не боится влажной уборки, но использовать подставку декоративную под водой мы не рекомендуем, достаточно протереть влажной тряпочкой, салфеткой. На изделии могут быть воздушные пузырьки, это не является браком, а наоборот придает изюминку.", "Может быть исполнена в разных техниках и цветах. Упаковывается в бумажный пакет.", "Изготовлено вручную." },
                    Price = 10,
                    Icon = images,
                    Sizes = new Dictionary<string, string?> {
                        { "Material","гипс" },
                        { "Weigth", null },
                        { "Height",null },
                        { "Depth",null },
                        { "Width", "9,5 см" },
                        { "Diameter",null },
                        { "Length","18 см" },  }
                };
                Variant variant1 = new Variant
                {
                    VariantId = 1,
                    Name = "Подсвечник",
                    Description = new List<string>(),
                    Price = 15,
                    Icon = images1,
                    ItemId = index,
                };
                Variant variant2 = new Variant
                {
                    VariantId = 2,
                    Name = "Шкатулка",
                    Description = new List<string> { "Шкатулка выполнена из гипсобетона, экологически безопасна и гипоаллергенна. Покрыта лаком. Ее можно использовать для хранения украшений или как подсвечник, установив в нее чайную свечу.", "Изготовлено вручную. Цвет и дизайн возможен любой." },
                    Price = 20,
                    Icon = images3,
                    ItemId = index,
                };
                Kit kit = new Kit
                {
                    KitId = 1,
                    Name = "Подсвечник + шкатулка",
                    Price = 25,
                    Variants = new List<int>() { 1, 2 },
                    Icon = images2,
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
                List<string> images1 = new() { "https://api.native-flora.tk/media/get/lilu?name=1_main.png", "https://api.native-flora.tk/media/get/lilu?name=1_second.png", "https://api.native-flora.tk/media/get/lilu?name=1_third.png" };
                List<string> images2 = new() { "https://api.native-flora.tk/media/get/lilu?name=2_main.png", "https://api.native-flora.tk/media/get/lilu?name=2_second.png", "https://api.native-flora.tk/media/get/lilu?name=2_third.png" };
                List<string> images3 = new() { "https://api.native-flora.tk/media/get/lilu?name=3_main.png", "https://api.native-flora.tk/media/get/lilu?name=3_second.png", "https://api.native-flora.tk/media/get/lilu?name=3_third.png" };
                List<string> images4 = new() { "https://api.native-flora.tk/media/get/lilu?name=4_main.png", "https://api.native-flora.tk/media/get/lilu?name=4_second.png", "https://api.native-flora.tk/media/get/lilu?name=4_third.png" };
                var item = new Item
                {
                    Id = index,
                    Name = "Лилу",
                    Description = new List<string> { "Декоративное кашпо «Лилу» из высокопрочного гипса. Покрыто моющейся краской.", "Изготовлено вручную.", "Варианты использования: 1. Подставка для кистей и косметики; 2. Кашпо для цветов и сухоцветов; 3. Подставка для столовых приборов; 4. Органайзер для карандашей, ручек; 5. Ваза для конфет.", "Шарик съемный, находится внутри кашпо, крепится отдельно.", "Каждое изделие упаковывается в отдельную коробку.", "Не наливайте воду в изделие, используйте дополнительную емкость!", "Используйте цветы, не требующие обильного полива.", "Цвет и дизайн возможен любой." },
                    Price = 55,
                    Icon = images1,
                    Sizes = new Dictionary<string, string?> {
                        { "Material","гипс" },
                        { "Weigth", "2,1 кг" },
                        { "Height","21 см" },
                        { "Depth","7 см" },
                        { "Diameter","8 см" },
                        { "Width", null },
                        { "Length",null },
                }
                };
                Variant variant1 = new Variant
                {
                    VariantId = 1,
                    Name = "Стрелки",
                    Description = new List<string>(),
                    Price = 58,
                    Icon = images2,
                    ItemId = index,
                };
                Variant variant2 = new Variant
                {
                    VariantId = 2,
                    Name = "Шипы",
                    Description = new List<string>(),
                    Price = 60,
                    Icon = images3,
                    ItemId = index,
                };
                Variant variant3 = new Variant
                {
                    VariantId = 3,
                    Name = "Патчи",
                    Description = new List<string>(),
                    Price = 60,
                    Icon = images4,
                    ItemId = index,
                };
                Kit kit1 = new Kit
                {
                    KitId = 1,
                    Name = "Стрелки + Шипы",
                    Price = 60,
                    Variants = new List<int>() { 1, 2 },
                    Icon = images3,
                    ItemId = index,
                };
                Kit kit2 = new Kit
                {
                    KitId = 2,
                    Name = "Патчи + Шипы",
                    Price = 60,
                    Variants = new List<int>() { 2, 3 },
                    Icon = new List<string> { "https://api.native-flora.tk/media/get/lilu?name=4_third.png", "https://api.native-flora.tk/media/get/lilu?name=3_main.png", "https://api.native-flora.tk/media/get/lilu?name=4_main.png" },
                    ItemId = index,
                };
                Kit kit3 = new Kit
                {
                    KitId = 3,
                    Name = "Стрелки + Патчи",
                    Price = 60,
                    Variants = new List<int>() { 1, 3 },
                    Icon = new List<string> { "https://api.native-flora.tk/media/get/lilu?name=4_third.png", "https://api.native-flora.tk/media/get/lilu?name=4_second.png" },

                    ItemId = index,
                };
                Kit kit4 = new Kit
                {
                    KitId = 4,
                    Name = "Стрелки + Патчи + шипы",
                    Price = 60,
                    Variants = new List<int>() { 1, 2, 3 },
                    Icon = new List<string> { "https://api.native-flora.tk/media/get/lilu?name=4_third.png", "https://api.native-flora.tk/media/get/lilu?name=4_second.png", "https://api.native-flora.tk/media/get/lilu?name=3_third.png" },

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
            if (!ItemExists(index))
            {
                var item = new Item
                {
                    Id = index,
                    Name = "Берни",
                    Description = new List<string> { "Органайзер «Берни» выполнен из высокопрочного гипса. Дизайнерский, оригинальный подарок на любой праздник или мероприятие. Можно использовать как ключницу, органайзер под различную мелочь, конфетницу или просто как интерьерное украшение.", "Изготовлено вручную. Каждое изделие упаковывается в отдельную коробку.", "Не наливайте воду в изделие!", "Цвет и дизайн возможен любой." },
                    Price = 45,
                    Sizes = new Dictionary<string, string?> {
                        { "Material","гипс" },
                        { "Weigth", "1,1 кг" },
                        { "Height","14 см" },
                        { "Depth","12 см" },
                        { "Width", "20 см" },
                        { "Diameter",null },
                        { "Length",null },
                    }
                };
                Variant variant1 = new Variant
                {
                    VariantId = 1,
                    Name = "Шипы",
                    Description = new List<string>(),
                    Price = 50,
                    ItemId = index,
                };
                _context.Items.Add(item);
                _context.SaveChanges();
                _context.Variants.Add(variant1);
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
    }
}