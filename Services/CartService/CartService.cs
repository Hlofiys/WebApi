using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using WebApi.Dtos.Cart;
using WebApi.Models;

namespace WebApi.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly TokenService _tokenService;
        public CartService(DataContext context, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
            _tokenService = new TokenService(_context, _configuration);

        }
        public async Task<ServiceResponse<string>> Add(int itemId, int itemAmount, int[] Variants, HttpRequest request)
        {
            var response = new ServiceResponse<string>();
            var token = request.Headers["x-access-token"].ToString();
            var UserSearchResult = _tokenService.UserSearch(token);
            if (UserSearchResult!.StatusCode == 401)
            {
                response.StatusCode = 401;
                response.Success = false;
                return response;
            }
            if (UserSearchResult!.Success == false)
            {
                response.Success = false;
                return response;
            }
            var user = UserSearchResult.Data!;
            var item = _context.Items.ToList().Find(u => u.Id == itemId);

            if (item is null)
            {
                response.Success = false;
                return response;
            }
            Cart cart;
            bool cartExists;
            if (user.CartId is null)
            {
                cartExists = false;
                cart = new Cart();
            }
            else
            {
                cart = _context.Carts.ToList().Find(c => c.Id == user.CartId)!;
                if (cart is null)
                {
                    cartExists = false;
                    cart = new Cart();
                }
                else
                {
                    cartExists = true;
                    var CartItems = _context.CartItems.ToList().FindAll(c => c.CartId == cart.Id && c.ItemId == itemId);
                    foreach (var CartItem in CartItems)
                    {
                        if (CartItem is not null)
                        {
                            if (CartItem.Variants is null && CartItem.Kit is not null &&
                               (bool)_context.Kits.ToList().Find(k => k.KitId == CartItem.Kit && k.ItemId == item.Id)?.Variants!.SequenceEqual(Variants.ToList())!)
                            {
                                response.Success = false;
                                response.Message = "This item is already in the cart";
                                return response;
                            }
                            if (CartItem.Variants is not null && CartItem.Variants!.SequenceEqual(Variants))
                            {
                                response.Success = false;
                                response.Message = "This item is already in the cart";
                                return response;
                            }
                        }
                    }


                }
            }
            bool VariantExists;
            int TotalVariantsPrice = 0;
            Kit? Kit = null;
            List<Variant> FoundVariants = new();
            if (Variants is not null)
            {
                List<Kit> kits = _context.Kits.ToList().FindAll(k => k.ItemId == itemId);
                foreach (var kit in kits)
                {

                    if (kit?.Variants?.ToArray().Length == Variants.Length &&
                        Variants.SequenceEqual(kit.Variants.ToArray()))
                    {
                        FoundVariants?.Add(_mapper.Map<Variant>(kit));
                        Kit = kit;
                    }
                }
                if (FoundVariants.Count == 0)
                {
                    Kit = null;
                    foreach (var variant in Variants)
                    {
                        if (variant > 0)
                        {
                            var FoundVariant = _context.Variants.ToList().Find(v => v.VariantId == variant && v.ItemId == item.Id)!;
                            if (FoundVariant is null)
                            {
                                response.Success = false;
                                response.Message = "This variant does not exist";
                                return response;
                            }
                            FoundVariants?.Add(FoundVariant);
                            if (!FoundVariants!.Any())
                            {
                                response.Success = false;
                                response.Message = "This variant does not exist";
                                return response;
                            }
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "This variant does not exist";
                            return response;
                        }
                    }
                }

                foreach (var variant in FoundVariants!)
                {
                    TotalVariantsPrice += (int)variant.Price!;
                }
                VariantExists = true;
            }
            else
            {
                VariantExists = false;
            }
            int ItemPrice = 0;
            if (cartExists)
            {
                if (!VariantExists)
                {
                    ItemPrice = (int)(item.Price * itemAmount)!;
                    if (cart.TotalPrice == 0)
                    {
                        cart.TotalPrice = (int)(item.Price * itemAmount)!;
                    }
                    else
                    {
                        cart.TotalPrice += (int)item.Price! * itemAmount;
                    }
                }
                else
                {
                    ItemPrice = TotalVariantsPrice * itemAmount;
                    if (cart.TotalPrice == 0)
                    {
                        cart.TotalPrice = TotalVariantsPrice * itemAmount;
                    }
                    else
                    {
                        cart.TotalPrice += TotalVariantsPrice * itemAmount;
                    }
                }


                _context.Update(cart);
            }
            else
            {
                _context.Carts.Add(cart);
                _context.SaveChanges();
                if (!VariantExists)
                {
                    ItemPrice = (int)(item.Price * itemAmount)!;
                    if (cart.TotalPrice == 0)
                    {
                        cart.TotalPrice = (int)(item.Price * itemAmount)!;
                    }
                    else
                    {
                        cart.TotalPrice += (int)(item.Price * itemAmount)!;
                    }
                }
                else
                {
                    ItemPrice = TotalVariantsPrice * itemAmount;
                    if (cart.TotalPrice == 0)
                    {
                        cart.TotalPrice = TotalVariantsPrice * itemAmount;
                    }
                    else
                    {
                        cart.TotalPrice += TotalVariantsPrice * itemAmount;
                    }
                }
                _context.Update(cart);
                user.CartId = cart.Id;
                _context.Update(user);
            }

            CartItem cartItem = new()
            {
                Amount = itemAmount,
                CartId = cart.Id,
                ItemId = item.Id,
                Price = ItemPrice,
            };
            if (Kit is not null)
            {
                cartItem.Kit = Kit.KitId;
            }
            else
            {
                cartItem.Variants = Variants;
            }

            _context.CartItems.Add(cartItem);

            _context.SaveChanges();

            response.Data = "";
            return response;
        }

        public async Task<ServiceResponse<CartAllDto>> All(HttpRequest request)
        {
            var response = new ServiceResponse<CartAllDto>();
            var token = request.Headers["x-access-token"].ToString();
            var UserSearchResult = _tokenService.UserSearch(token);
            if (UserSearchResult!.StatusCode == 401)
            {
                response.StatusCode = 401;
                response.Success = false;
                return response;
            }
            if (UserSearchResult!.Success == false)
            {
                response.Success = false;
                return response;
            }
            var user = UserSearchResult.Data!;
            if (user.CartId is null)
            {
                response.Message = "The user has no products";
                return response;
            }
            var cart = _context.Carts.ToList().Find(c => c.Id == user.CartId);
            if (cart is null)
            {
                response.Message = "The user has no products";
                return response;
            }
            var CartItems = _context.CartItems.ToList().FindAll(c => c.CartId == cart.Id);
            if (CartItems is null || CartItems.Count == 0)
            {
                response.Message = "The user has no products";
                return response;
            }
            List<CartItemDto> itemDtos = new();
            foreach (var item in CartItems)
            {
                var FoundItem = _context.Items.ToList().Find(i => i.Id == item.ItemId);
                if (FoundItem is null) continue;
                List<Variant>? FoundVariants = new();
                Kit? FoundKit = null;
                if (item.Variants is not null)
                {
                    foreach (var variant in item.Variants)
                    {
                        FoundVariants.Add(_context.Variants.ToList().Find(v => v.VariantId == variant && v.ItemId == item.ItemId)!);
                    }
                    if (!FoundVariants.Any())
                    {
                        FoundVariants = null;
                    }

                }
                else FoundVariants = null;

                if (FoundVariants is null && item.Variants is not null && item.Variants.Length > 0)
                {
                    response.Success = false;
                    response.Message = "Error finding product variant";
                    return response;
                }

                if (item.Kit is not null)
                {
                    FoundKit = _context.Kits.ToList().Find(k => k.KitId == item.Kit && k.ItemId == item.ItemId);
                };

                if (FoundKit is null && item.Kit is not null)
                {
                    response.Success = false;
                    response.Message = "Error finding product kit";
                    return response;
                }
                List<VariantDto>? variantDto = new();
                if (FoundVariants is not null)
                {
                    foreach (var var in FoundVariants)
                    {
                        variantDto.Add(_mapper.Map<VariantDto>(var));
                    }
                }
                else
                {
                    variantDto = null;
                }
                CartItemDto itemDto = new()
                {
                    CartItemId = item.Id,
                    Item = FoundItem,
                    Amount = item.Amount,
                    Price = item.Price,
                    Variants = variantDto switch
                    {
                        null => null,
                        _ => variantDto.ToArray(),
                    },
                    Kit = _mapper.Map<KitDto>(FoundKit),
                };
                itemDtos.Add(itemDto);

            }
            if (itemDtos.Count == 0)
            {
                response.Success = false;
                response.Message = "Product search error";
                return response;
            }
            CartAllDto cartAll = new()
            {
                CartItems = itemDtos,
                TotalPrice = cart.TotalPrice!,
            };
            response.Data = cartAll;
            return response;
        }

        public async Task<ServiceResponse<int>> Count(HttpRequest request)
        {
            var response = new ServiceResponse<int>();
            var token = request.Headers["x-access-token"].ToString();
            var UserSearchResult = _tokenService.UserSearch(token);
            if (UserSearchResult!.StatusCode == 401)
            {
                response.StatusCode = 401;
                response.Success = false;
                return response;
            }
            if (UserSearchResult!.Success == false)
            {
                response.Success = false;
                return response;
            }
            var user = UserSearchResult.Data!;
            if (user.CartId is null)
            {
                response.Data = 0;
                return response;
            }
            var cart = _context.Carts.ToList().Find(c => c.Id == user.CartId);
            if (cart is null)
            {
                response.Data = 0;
                return response;
            }
            var CartItems = _context.CartItems.ToList().FindAll(c => c.CartId == cart.Id);
            if (CartItems is null)
            {
                response.Data = 0;
                return response;
            }
            response.Data = CartItems.Count;
            return response;
        }

        public async Task<ServiceResponse<string>> Delete(int InCartId, int[] Variants, HttpRequest request)
        {
            var response = new ServiceResponse<string>();
            var token = request.Headers["x-access-token"].ToString();
            var UserSearchResult = _tokenService.UserSearch(token);
            if (UserSearchResult!.StatusCode == 401)
            {
                response.StatusCode = 401;
                response.Success = false;
                return response;
            }
            if (UserSearchResult!.Success == false)
            {
                response.Success = false;
                return response;
            }
            var user = UserSearchResult.Data!;
            if (user.CartId is null)
            {
                response.Success = false;
                response.Message = "The user has no products";
                return response;
            }
            var cart = _context.Carts.ToList().Find(c => c.Id == user.CartId);
            if (cart is null)
            {
                response.Success = false;
                response.Message = "The user has no products";
                return response;
            }
            var CartItem = _context.CartItems.ToList().Find(c => c.CartId == cart.Id && c.Id == InCartId);
            if (CartItem is null)
            {
                response.Success = false;
                response.Message = "The user does not have a product with this id";
                return response;
            }
            var Id = CartItem?.ItemId;

            if (Variants is not null)
            {
                if (CartItem?.Variants?.Length > 0)
                {
                    int OriginalVariantsLength = CartItem.Variants.Length;
                    foreach (var variant in CartItem?.Variants!)
                    {
                        if (Variants.Contains(variant))
                        {
                            cart.TotalPrice -= (int)_context.Variants.ToList().Find(v => v.VariantId == variant && v?.ItemId == Id)?.Price! * (int)CartItem?.Amount!;
                            CartItem!.Variants = CartItem?.Variants!.Where(v => v != variant).ToArray();

                        }
                        if (CartItem?.Variants is null || CartItem?.Variants.Length == 0)
                        {
                            cart.TotalPrice += (int)_context.Items.ToList().Find(i => i.Id == CartItem?.ItemId)?.Price! * (int)CartItem?.Amount!;
                            CartItem!.Price = (int)(_context.Items.ToList().Find(i => i.Id == CartItem?.ItemId)?.Price * CartItem?.Amount)!;
                            break;
                        }
                    }
                    if (OriginalVariantsLength != CartItem?.Variants?.Length)
                    {
                        if (CheckForExists(CartItem!, cart) == true)
                        {
                            return response;
                        }
                        else
                        {
                            response.StatusCode = 400;
                            response.Success = false;
                            response.Message = "This CartItem already exists";
                            return response;
                        }
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "The product does not have such options";
                        return response;
                    }
                }
                else if (CartItem?.Kit is not null)
                {
                    List<int> NewVariants = new();
                    var Kit = _context.Kits.ToList().Find(k => k.KitId == CartItem?.Kit && k.ItemId == CartItem?.ItemId);
                    if (Kit is not null)
                    {
                        foreach (var variant in Kit?.Variants!)
                        {
                            if (!Variants.Contains(variant))
                            {
                                NewVariants.Add(variant);
                            }
                        }
                        if (NewVariants.Count > 0)
                        {
                            List<Kit> kits = _context.Kits.ToList().FindAll(k => k.ItemId == CartItem?.ItemId);
                            Kit? NewKit = null;
                            foreach (var kit in kits)
                            {

                                if (kit?.Variants?.ToArray().Length == NewVariants.ToArray().Length)
                                {
                                    if (NewVariants.ToArray().SequenceEqual(kit.Variants.ToArray()))
                                    {
                                        NewKit = kit;
                                    }
                                }
                            }

                            if (NewKit is not null)
                            {
                                cart.TotalPrice -= (int)Kit.Price! * CartItem.Amount;
                                CartItem.Price = (int)NewKit.Price! * CartItem.Amount;
                                cart.TotalPrice += (int)NewKit.Price! * CartItem.Amount;
                                CartItem.Kit = NewKit.KitId;
                                CartItem.Variants = null;
                                if (CheckForExists(CartItem, cart) == true)
                                {
                                    return response;
                                }
                                else
                                {
                                    response.StatusCode = 400;
                                    response.Message = "This CartItem already exists";
                                    response.Success = false;
                                    return response;
                                }

                            }
                            int TotalPrice = 0;
                            foreach (var variant in NewVariants)
                            {
                                TotalPrice += (int)_context.Variants.ToList().Find(v => v.VariantId == variant && v.ItemId == CartItem?.ItemId)?.Price! * CartItem.Amount;
                            }
                            cart.TotalPrice -= (int)Kit.Price! * CartItem.Amount;
                            CartItem.Price = TotalPrice;
                            cart.TotalPrice += TotalPrice;
                            CartItem.Kit = null;
                            CartItem.Variants = NewVariants.ToArray();
                            if (CheckForExists(CartItem, cart) == true)
                            {
                                return response;
                            }
                            else
                            {
                                response.StatusCode = 400;
                                response.Message = "This CartItem already exists";
                                response.Success = false;
                                return response;
                            }

                        }
                        else if (NewVariants.Count == 0)
                        {
                            cart.TotalPrice -= (int)Kit.Price! * CartItem.Amount;
                            cart.TotalPrice += (int)_context.Items.ToList().Find(i => i.Id == CartItem?.ItemId)?.Price! * (int)CartItem.Amount;
                            CartItem.Price = (int)(_context.Items.ToList().Find(i => i.Id == CartItem?.ItemId)?.Price * CartItem.Amount)!;
                            CartItem.Kit = null;
                            CartItem.Variants = null;
                            if (CheckForExists(CartItem, cart) == true)
                            {
                                return response;
                            }
                            else
                            {
                                response.StatusCode = 400;
                                response.Success = false;
                                response.Message = "This CartItem already exists";
                                return response;
                            }
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "Error";
                            return response;
                        }
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Error in DataBase";
                        return response;
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "Product doesn't have such variants";
                    return response;
                }
            }
            else
            {
                if (CartItem.Kit != null)
                {
                    cart.TotalPrice -= (int)_context.Kits.ToList().Find(k => k.KitId == CartItem?.Kit && k.ItemId == CartItem.ItemId)?.Price! * CartItem.Amount;
                    _context.CartItems.Remove(CartItem);
                    _context.Carts.Update(cart);
                    _context.SaveChanges();
                    return response;
                }
                else if (CartItem.Variants != null)
                {
                    foreach (var variant in CartItem.Variants)
                    {
                        cart.TotalPrice -= (int)_context.Variants.ToList().Find(v => v.VariantId == variant && v.ItemId == CartItem.ItemId)?.Price! * CartItem.Amount;
                    }
                    _context.CartItems.Remove(CartItem);
                    _context.Carts.Update(cart);
                    _context.SaveChanges();
                    return response;
                }
                cart.TotalPrice -= (int)_context.Items.ToList().Find(i => i.Id == CartItem?.ItemId)?.Price! * CartItem.Amount;
                _context.CartItems.Remove(CartItem);
                _context.Carts.Update(cart);
                _context.SaveChanges();
                return response;
            }

            bool CheckForExists(CartItem cartItem, Cart cart)
            {
                try
                {
                    if (cartItem.Variants != null && cartItem.Variants!.Count() > 0)
                    {
                        var search = _context.CartItems.ToList().Find(c => c.Id != cartItem.Id && c.ItemId == cartItem.ItemId && c.Kit == null && c.Variants != null && cartItem!.Variants!.SequenceEqual(c.Variants))!.Id;
                    }
                    else if (cartItem.Kit != null)
                    {
                        var search = _context.CartItems.ToList().Find(c => c.Id != cartItem.Id && c.ItemId == cartItem.ItemId && c.Kit == cartItem.Kit)!.Id;
                    }
                    else
                    {
                        var search = _context.CartItems.ToList().Find(c => c.Id != cartItem.Id && c.ItemId == cartItem.ItemId && c.Variants == null && c.Kit == null)!.Id;
                    }
                }
                catch
                {
                    if (cartItem.Variants != null && cartItem.Variants.Count() == 0)
                    {
                        cartItem.Variants = null;
                    }
                    _context.CartItems.Update(cartItem);
                    _context.Carts.Update(cart);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }

        }

        public async Task<ServiceResponse<int>> PriceGet(int itemId, int itemAmount, int[] Variants)
        {
            var response = new ServiceResponse<int>();
            var item = _context.Items.ToList().Find(u => u.Id == itemId);

            if (item is null)
            {
                response.Success = false;
                return response;
            }
            bool VariantExists;
            int TotalVariantsPrice = (int)item.Price!;
            int TotalPrice = 0;
            Kit? Kit = null;
            List<Variant> FoundVariants = new();
            if (Variants is not null)
            {
                List<Kit> kits = _context.Kits.ToList().FindAll(k => k.ItemId == itemId);
                foreach (var kit in kits)
                {

                    if (kit?.Variants?.ToArray().Length == Variants.Length && Variants.SequenceEqual(kit.Variants.ToArray()))
                    {
                        FoundVariants?.Add(_mapper.Map<Variant>(kit));
                        Kit = kit;
                    }
                }
                if (FoundVariants.Count == 0)
                {
                    Kit = null;
                    foreach (var variant in Variants)
                    {
                        if (variant > 0)
                        {
                            var FoundVariant = _context.Variants.ToList().Find(v => v.VariantId == variant && v.ItemId == item.Id)!;
                            if (FoundVariant is null)
                            {
                                response.Success = false;
                                response.Message = "This variant does not exist";
                                return response;
                            }
                            FoundVariants?.Add(FoundVariant);
                            if (!FoundVariants!.Any())
                            {
                                response.Success = false;
                                response.Message = "This variant does not exist";
                                return response;
                            }
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "This variant does not exist";
                            return response;
                        }
                    }
                }

                foreach (var variant in FoundVariants!)
                {
                    TotalVariantsPrice += (int)variant.Price!;
                }
                VariantExists = true;
            }
            else
            {
                VariantExists = false;
            }
            int ItemPrice = 0;
            if (!VariantExists)
            {
                ItemPrice = (int)(item.Price * itemAmount)!;
                TotalPrice = (int)(item.Price * itemAmount)!;
            }
            else
            {
                ItemPrice = TotalVariantsPrice * itemAmount;
                TotalPrice = TotalVariantsPrice * itemAmount;
            }

            response.Data = TotalPrice;
            return response;
        }

        public async Task<ServiceResponse<CartAllDto>> Update(int CartItemId, int[]? Variants, int? Amount, HttpRequest request)
        {
            var response = new ServiceResponse<CartAllDto>();
            var token = request.Headers["x-access-token"].ToString();
            var UserSearchResult = _tokenService.UserSearch(token);
            if (UserSearchResult!.StatusCode == 401)
            {
                response.StatusCode = 401;
                response.Success = false;
                return response;
            }
            if (UserSearchResult!.Success == false)
            {
                response.Success = false;
                return response;
            }
            var user = UserSearchResult.Data!;
            if (user.CartId is null)
            {
                response.Success = false;
                response.Message = "The user has no products";
                return response;
            }
            var cart = _context.Carts.ToList().Find(c => c.Id == user.CartId);
            if (cart is null)
            {
                response.Success = false;
                response.Message = "The user has no products";
                return response;
            }
            var CartItem = _context.CartItems.ToList().Find(c => c.CartId == cart.Id && c.Id == CartItemId);
            if (CartItem is null)
            {
                response.Success = false;
                response.Message = "The user does not have a product with this id";
                return response;
            }
            var Id = CartItem.ItemId;
            // ���� �������� � ������� �������
            if (Variants is not null)
            {
                //���� � ��������� ������ � ������� ��������� �������� � ��� ���������� ���������� ������
                if (CartItem?.Variants is not null && CartItem?.Variants!.Length > 0)
                {
                    foreach (var variant in Variants)
                    {
                        if ((bool)(CartItem?.Variants!.Contains(variant))!)
                        {
                            response.Success = false;
                            response.Message = "This product already contains this variant";
                            return response;
                        }
                    }
                }
                else if (CartItem?.Kit is not null)
                {
                    var FilterKit = _context.Kits.ToList().Find(k => k.KitId == CartItem?.Kit && k.ItemId == Id);
                    foreach (var variant in Variants)
                    {
                        if ((bool)(FilterKit?.Variants!.Contains(variant)!))
                        {
                            response.Success = false;
                            response.Message = "This product already contains this variant";
                            return response;
                        }
                    }
                }
                // ��������� ������� �� ����� ���-�� ������, � ���� �� - ��������� ���� �������
                if (Amount is not null)
                {
                    cart.TotalPrice -= (int)CartItem?.Price!;
                    var TempPrice = CartItem?.Price / CartItem?.Amount;
                    TempPrice = (int)(TempPrice! * Amount);
                    CartItem!.Price = (int)TempPrice;
                    CartItem.Amount = (int)Amount!;
                    cart.TotalPrice += (int)CartItem?.Price!;
                }
                // ���� ��������� �������� ������ � ������� � ���������
                List<Variant> NewVariants = new();
                foreach (var variant in Variants)
                {
                    var foundVariant = _context.Variants.ToList().Find(v => v.VariantId == variant && v.ItemId == Id);
                    if (foundVariant is null)
                    {
                        response.Success = false;
                        response.Message = "Specified variant does not exists";
                        return response;
                    }
                    NewVariants.Add(foundVariant);
                }
                if (NewVariants.Count == 0)
                {
                    response.Success = false;
                    response.Message = "Error";
                    return response;
                }

                Kit kit = new();
                int[]? NewVariantsIds = null;

                if (CartItem?.Variants is not null)
                {
                    foreach (var variant in CartItem?.Variants!)
                    {
                        NewVariants.Add(_context.Variants.ToList().Find(v => v.VariantId == variant && v.ItemId == Id)!);
                    }

                    NewVariantsIds = Variants.ToArray().Union(CartItem?.Variants!).ToArray();
                    Array.Sort(NewVariantsIds);

                }
                else if (CartItem?.Kit is not null)
                {
                    kit = _context.Kits.ToList().Find(k => k.KitId == CartItem.Kit && k.ItemId == Id)!;
                    if (kit is null)
                    {
                        response.Success = false;
                        response.Message = "Error";
                        return response;
                    }
                    foreach (var variant in kit?.Variants!)
                    {
                        NewVariants.Add(_context.Variants.ToList().Find(v => v.VariantId == variant && v.ItemId == Id)!);
                    }

                    NewVariantsIds = Variants.ToArray().Union(kit?.Variants!).ToArray();
                    Array.Sort(NewVariantsIds);
                    if (NewVariantsIds.Length == 0)
                    {
                        response.Success = false;
                        response.Message = "Error";
                        return response;
                    }
                    Console.WriteLine(NewVariantsIds.Length);
                }
                else
                {
                    NewVariantsIds = Variants;
                    Array.Sort(NewVariantsIds);
                }

                List<Kit> kits = _context.Kits.ToList().FindAll(k => k.ItemId == CartItem?.ItemId);
                List<Variant> FoundVariants = new();
                Kit? Kit = null;
                foreach (var localkit in kits)
                {

                    if (localkit?.Variants?.ToArray().Length == NewVariantsIds?.Length)
                    {
                        if (NewVariantsIds!.SequenceEqual(localkit!.Variants!.ToArray()))
                        {
                            FoundVariants.Add(_mapper.Map<Variant>(localkit));
                            Kit = localkit;
                        }
                    }
                }
                if (FoundVariants.Count == 0)
                {
                    Kit = null;
                    foreach (var variant in Variants)
                    {
                        if (variant > 0)
                        {
                            FoundVariants?.Add(_context.Variants.ToList().Find(v => v.VariantId == variant && v.ItemId == CartItem?.ItemId)!);
                            if (!FoundVariants!.Any())
                            {
                                response.Success = false;
                                response.Message = "This option is not";
                                return response;
                            }
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "This option is not";
                            return response;
                        }
                    }
                }
                int TotalVariantsPrice = 0;

                foreach (var variant in FoundVariants!)
                {
                    TotalVariantsPrice += (int)variant.Price!;
                }
                int ItemPrice = 0;
                ItemPrice = (int)(TotalVariantsPrice * CartItem?.Amount)!;
                if (CartItem!.Kit is not null)
                {
                    cart.TotalPrice -= (int)(_context.Kits.ToList().Find(k => k.KitId == CartItem!.Kit && k.ItemId == CartItem!.ItemId)!.Price * CartItem?.Amount)!;
                }
                else if (CartItem!.Variants is not null)
                {
                    foreach (var variant in CartItem!.Variants)
                    {
                        cart.TotalPrice -= (int)(_context.Variants.ToList().Find(v => v.VariantId == variant && v.ItemId == CartItem!.ItemId)!.Price * CartItem?.Amount)!;
                    }
                }
                else
                {
                    cart.TotalPrice -= (int)(_context.Items.ToList().Find(i => i.Id == CartItem!.ItemId)!.Price * CartItem?.Amount)!;
                }

                cart.TotalPrice += (int)(TotalVariantsPrice * CartItem?.Amount)!;
                CartItem!.Price = (int)(TotalVariantsPrice * CartItem?.Amount)!;
                _context.Carts.Update(cart);

                if (Kit is not null)
                {
                    CartItem!.Kit = Kit.KitId;
                    CartItem!.Variants = null;
                }
                else
                {
                    CartItem!.Kit = null;
                    CartItem!.Variants = NewVariantsIds;
                }

                try
                {
                    if (CartItem.Variants != null && CartItem.Variants!.Count() > 0)
                    {
                        var search = _context.CartItems.ToList().Find(c => c.Id != CartItem.Id && c.ItemId == CartItem.ItemId && c.Kit == null && c.Variants != null && CartItem!.Variants!.SequenceEqual(c.Variants))!.Id;
                    }
                    else if (CartItem.Kit != null)
                    {
                        var search = _context.CartItems.ToList().Find(c => c.Id != CartItem.Id && c.ItemId == CartItem.ItemId && c.Kit == CartItem.Kit)!.Id;
                    }
                    else
                    {
                        var search = _context.CartItems.ToList().Find(c => c.Id != CartItem.Id && c.ItemId == CartItem.ItemId && c.Variants == null && c.Kit == null)!.Id;
                    }
                }
                catch
                {
                    _context.CartItems.Update(CartItem);
                    _context.SaveChanges();

                    var all = await All(request);

                    response.Data = all.Data;

                    return response;
                }
                response.StatusCode = 400;
                response.Message = "This CartItem already exists";
                response.Success = false;
                return response;

            }
            else if (Amount is not null)
            {
                cart.TotalPrice -= (int)CartItem?.Price!;
                var TempPrice = CartItem?.Price / CartItem?.Amount;
                TempPrice = (int)(TempPrice! * Amount);
                CartItem!.Price = (int)TempPrice;
                CartItem.Amount = (int)Amount!;
                cart.TotalPrice += (int)CartItem?.Price!;
                _context.Carts.Update(cart);
                _context.CartItems.Update(CartItem!);
                _context.SaveChanges();
                var all = await All(request);

                response.Data = all.Data;

                return response;
            }
            else
            {
                response.Success = false;
                response.Message = "Specify fields for updating";
                return response;
            }
        }
    }
}