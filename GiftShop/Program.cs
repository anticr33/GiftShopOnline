using GiftShop.Data; // За достъп до контекста на базата данни
using Microsoft.AspNetCore.Identity; // За Identity услугите
using Microsoft.EntityFrameworkCore; // За Entity Framework Core

namespace GiftShop
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Добавяне на връзка към базата данни
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Настройки за Identity
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.Password.RequireDigit = true; // Изисква поне една цифра
                options.Password.RequiredLength = 6; // Минимална дължина на паролата
                options.Password.RequireNonAlphanumeric = false; // Не изисква специални символи
                options.Password.RequireUppercase = true; // Изисква главна буква
                options.Password.RequireLowercase = true; // Изисква малка буква
                options.SignIn.RequireConfirmedAccount = true; // Изисква потвърждение на акаунта
            })
            .AddRoles<IdentityRole>() // Добавя поддръжка за роли
            .AddEntityFrameworkStores<ApplicationDbContext>();

            // Добавяне на MVC и Razor Pages
            builder.Services.AddControllersWithViews();


            var app = builder.Build(); 
            var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await SeedRolesAsync(roleManager);



            // Конфигурация на HTTP заявките
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint(); // Използване на автоматично приложение на миграции в разработка
            }
            else
            {
                app.UseExceptionHandler("/Home/Error"); // Глобална обработка на грешки
                app.UseHsts(); // Настройка на HTTP Strict Transport Security
            }

            app.UseHttpsRedirection(); // Пренасочване към HTTPS
            app.UseStaticFiles(); // Разрешаване на статични файлове

            app.UseRouting(); // Активиране на маршрутизиране

            app.UseAuthentication(); // Активиране на автентикация
            app.UseAuthorization(); // Активиране на авторизация

            // Персонализиран маршрут за продукти
            app.MapControllerRoute(
               name: "productDetails",
               pattern: "Products/Details/{id?}",
               defaults: new { controller = "Products", action = "Details" });

            // Основен маршрут
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages(); // Поддържане на Razor Pages за Identity

            app.Run(); // Стартиране на приложението
        }
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
        }
        public static async Task SeedAdminUserAsync(UserManager<IdentityUser> userManager)
        {
            var adminUser = await userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null)
            {
                var newUser = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com" };
                await userManager.CreateAsync(newUser, "Admin123!");
                await userManager.AddToRoleAsync(newUser, "Admin");
            }
        }

    }
}
