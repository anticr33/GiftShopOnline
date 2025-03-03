
using System.Collections.Generic;

namespace GiftShop.Models
{
    public class StatisticsViewModel
    {
        public decimal TotalRevenue { get; set; } // Общ оборот
        public decimal TotalProfit { get; set; }  // Печалба
        public List<Order> Orders { get; set; } = new List<Order>(); // Списък с поръчки
    }
}
