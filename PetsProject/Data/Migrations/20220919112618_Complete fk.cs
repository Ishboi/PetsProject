using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetsProject.Data.Migrations
{
    public partial class Completefk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "CategoriesPets",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesPets", x => new { x.CategoriesId, x.PetsId });
                    table.ForeignKey(
                        name: "FK_CategoriesPets_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoriesPets_Pets_PetsId",
                        column: x => x.PetsId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesPets_PetsId",
                table: "CategoriesPets",
                column: "PetsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriesPets");

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
    }
}
