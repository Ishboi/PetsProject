using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetsProject.Data.Migrations
{
    public partial class PetsCategoriesforeignkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PetsCategoriesId",
                table: "Pets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PetsCategoriesId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PetsCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetsCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pets_PetsCategoriesId",
                table: "Pets",
                column: "PetsCategoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_PetsCategoriesId",
                table: "Categories",
                column: "PetsCategoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_PetsCategories_PetsCategoriesId",
                table: "Categories",
                column: "PetsCategoriesId",
                principalTable: "PetsCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_PetsCategories_PetsCategoriesId",
                table: "Pets",
                column: "PetsCategoriesId",
                principalTable: "PetsCategories",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_PetsCategories_PetsCategoriesId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Pets_PetsCategories_PetsCategoriesId",
                table: "Pets");

            migrationBuilder.DropTable(
                name: "PetsCategories");

            migrationBuilder.DropIndex(
                name: "IX_Pets_PetsCategoriesId",
                table: "Pets");

            migrationBuilder.DropIndex(
                name: "IX_Categories_PetsCategoriesId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PetsCategoriesId",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "PetsCategoriesId",
                table: "Categories");
        }
    }
}
