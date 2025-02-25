using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftShop.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string? SKU { get; set; }  // ✅ Добавено SKU

        [Required]
        public string ProductName { get; set; }  // ✅ Добавено име на продукта

        [Required]
        public decimal Price { get; set; }  // ✅ Добавена цена на продукта

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
