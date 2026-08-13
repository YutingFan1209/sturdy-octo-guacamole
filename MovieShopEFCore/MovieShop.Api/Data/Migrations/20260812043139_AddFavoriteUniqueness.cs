using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieShop.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Favorites_MovieId",
                table: "Favorites");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_MovieId_UserId",
                table: "Favorites",
                columns: new[] { "MovieId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Favorites_MovieId_UserId",
                table: "Favorites");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_MovieId",
                table: "Favorites",
                column: "MovieId");
        }
    }
}
