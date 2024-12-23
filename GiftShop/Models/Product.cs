﻿namespace GiftShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }

        public int CategoryId { get; set; } // Външен ключ към категория
        public Category Category { get; set; } // Навигационно свойство
        public string Occasion { get; set; } // Нова колона за повода

    }
}