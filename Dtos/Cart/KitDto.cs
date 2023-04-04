﻿namespace WebApi.Dtos.Cart
{
    public class KitDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Description { get; set; } = new List<string>();
        public List<int>? Variants { get; set; } = null;
        public int? Price { get; set; } = null;
        public List<string>? Icon { get; set; } = null;
        public int? ItemId { get; set; } = null;
        public string Video { get; set; } = string.Empty;
    }
}
