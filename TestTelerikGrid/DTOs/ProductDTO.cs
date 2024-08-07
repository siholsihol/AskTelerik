﻿namespace TestTelerikGrid.DTOs
{
    public class ProductDTO
    {
        public bool Selected { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public bool Active { get; set; }
        public int CategoryId { get; set; }
    }
}
