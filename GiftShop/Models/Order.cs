using GiftShop.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GiftShop.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string? UserId { get; set; } // Позволяваме NULL за гости
        public string SessionId { get; set; } = Guid.NewGuid().ToString(); // Генерираме стойност по подразбиране за гости

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public string ShippingAddress { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string ShippingMethod { get; set; } // Еконт или Спиди

        [Required]
        public string PaymentMethod { get; set; } // Карта или наложен платеж

        public string FullName { get; set; } // Добавено поле за име и фамилия
        public bool IsCompleted { get; set; } = false;

        public string Status { get; set; } = "Pending"; // Статус по подразбиране

        public ICollection<OrderItem> OrderItems { get; set; }
        public decimal TotalPrice { get; internal set; }
    }
}