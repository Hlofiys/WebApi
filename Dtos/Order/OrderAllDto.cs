﻿using WebApi.Dtos.Cart;

namespace WebApi.Dtos.Order
{
    public class OrderAllDto
    {
        public bool? Shipping { get; set; } = null;
        public string Address { get; set; } = string.Empty;
        public string FIO { get; set; } = string.Empty;
        public string PhoneNubmer { get; set; } = string.Empty;
        public string? Contact { get; set; } = string.Empty;
        public List<CartItemDto>? CartItems { get; set; } = null;
        public int TotalPrice { get; set; } = 0;
    }
}
