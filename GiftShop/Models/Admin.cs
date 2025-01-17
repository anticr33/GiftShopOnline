
using System.ComponentModel.DataAnnotations;

namespace GiftShop.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
