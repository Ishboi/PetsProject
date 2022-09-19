using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetsProject.Data.Migrations
{
    public partial class changingforeignkeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_PetsCategories_PetsCategoriesId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Pets_PetsCategories_PetsCategoriesId",
                table: "Pets");

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

            migrationBuilder.AddColumn<Guid>(
                name: "CategoriesId",
                table: "PetsCategories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PetsId",
                table: "PetsCategories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PetsCategories_CategoriesId",
                table: "PetsCategories",
                column: "CategoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_PetsCategories_PetsId",
                table: "PetsCategories",
                column: "PetsId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PetsCategories_Categories_CategoriesId",
                table: "PetsCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_PetsCategories_Pets_PetsId",
                table: "PetsCategories");

            migrationBuilder.DropIndex(
                name: "IX_PetsCategories_CategoriesId",
                table: "PetsCategories");

            migrationBuilder.DropIndex(
                name: "IX_PetsCategories_PetsId",
                table: "PetsCategories");

            migrationBuilder.DropColumn(
                name: "CategoriesId",
                table: "PetsCategories");

            migrationBuilder.DropColumn(
                name: "PetsId",
                table: "PetsCategories");

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
    }
}
