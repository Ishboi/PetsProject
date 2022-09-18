using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetsProject.Data.Migrations
{
    public partial class addcategoriestoPetsModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PetsId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_PetsId",
                table: "Categories",
                column: "PetsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Pets_PetsId",
                table: "Categories",
                column: "PetsId",
                principalTable: "Pets",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Pets_PetsId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_PetsId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PetsId",
                table: "Categories");
        }
    }
}
