using GiftShop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GiftShop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Admin> Admins { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Задаване на прецизност за Price колоната
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18, 2)");

            // Добавяне на примерни данни за категории с полето ImageUrl
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "За Него", Description = "Смели подаръци за мъже", ImageUrl = "/images/ForHim1.png" },
                new Category { Id = 2, Name = "За Нея", Description = "Прекрасни подаръци за жени", ImageUrl = "/images/ForHer.png" },
                new Category { Id = 3, Name = "За Деца", Description = "Играчки и забавления за най-малките", ImageUrl = "/images/ForKids.png" },
                new Category { Id = 4, Name = "За Дома", Description = "Подаръци за уют и декорация на дома", ImageUrl = "/images/ForHome.png" },
                new Category { Id = 5, Name = "Персонализирани", Description = "Уникални персонализирани подаръци", ImageUrl = "/images/personalized.png" },
                new Category { Id = 6, Name = "Хобита", Description = "Подаръци за всякакви хобита и интереси", ImageUrl = "/images/hobies.png" }
            );
        }
    }
}
