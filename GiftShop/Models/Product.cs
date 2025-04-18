﻿using System.ComponentModel.DataAnnotations;

namespace GiftShop.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; } // Foreign key
        public Category Category { get; set; }


        public string? SKU { get; set; }
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Purchase price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Purchase price must be greater than 0")]
        public decimal PurchasePrice { get; set; } // Цена на закупуване

    }

}
