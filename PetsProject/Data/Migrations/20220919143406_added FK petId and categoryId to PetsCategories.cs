using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetsProject.Data.Migrations
{
    public partial class addedFKpetIdandcategoryIdtoPetsCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PetsCategories_Categories_CategoriesId",
                table: "PetsCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_PetsCategories_Pets_PetsId",
                table: "PetsCategories");

            migrationBuilder.RenameColumn(
                name: "PetsId",
                table: "PetsCategories",
                newName: "PetId");

            migrationBuilder.RenameColumn(
                name: "CategoriesId",
                table: "PetsCategories",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_PetsCategories_PetsId",
                table: "PetsCategories",
                newName: "IX_PetsCategories_PetId");

            migrationBuilder.RenameIndex(
                name: "IX_PetsCategories_CategoriesId",
                table: "PetsCategories",
                newName: "IX_PetsCategories_CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_PetsCategories_Categories_CategoryId",
                table: "PetsCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PetsCategories_Pets_PetId",
                table: "PetsCategories",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PetsCategories_Categories_CategoryId",
                table: "PetsCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_PetsCategories_Pets_PetId",
                table: "PetsCategories");

            migrationBuilder.RenameColumn(
                name: "PetId",
                table: "PetsCategories",
                newName: "PetsId");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "PetsCategories",
                newName: "CategoriesId");

            migrationBuilder.RenameIndex(
                name: "IX_PetsCategories_PetId",
                table: "PetsCategories",
                newName: "IX_PetsCategories_PetsId");

            migrationBuilder.RenameIndex(
                name: "IX_PetsCategories_CategoryId",
                table: "PetsCategories",
                newName: "IX_PetsCategories_CategoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_PetsCategories_Categories_CategoriesId",
                table: "PetsCategories",
                column: "CategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PetsCategories_Pets_PetsId",
                table: "PetsCategories",
                column: "PetsId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
