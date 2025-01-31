using X.PagedList;

namespace GiftShop.Models
{
    public class CategoryViewModel
    {
        public Category Category { get; set; }
        public IPagedList<Product> PagedProducts { get; set; }
    }
}
