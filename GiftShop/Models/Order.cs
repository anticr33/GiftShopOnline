using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GiftShop.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // Свързване с потребителя

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

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
