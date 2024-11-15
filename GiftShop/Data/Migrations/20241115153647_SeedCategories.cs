using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GiftShop.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, "Смели подаръци за мъже", "/images/ForHim1.png", "За Него" },
                    { 2, "Прекрасни подаръци за жени", "/images/ForHer.png", "За Нея" },
                    { 3, "Играчки и забавления за най-малките", "/images/ForKids.png", "За Деца" },
                    { 4, "Подаръци за уют и декорация на дома", "/images/ForHome.png", "За Дома" },
                    { 5, "Уникални персонализирани подаръци", "/images/personalized.png", "Персонализирани" },
                    { 6, "Подаръци за всякакви хобита и интереси", "/images/hobies.png", "Хобита" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
