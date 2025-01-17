using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace GiftShop.Models
{
    public class Category
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }  // Ново поле за URL на изображението
        public List<Product> Products { get; set; }
    }

}
