using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mockups.Migrations
{
    /// <inheritdoc />
    public partial class AddProductViewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MenuItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductViews_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductViews_MenuItemId",
                table: "ProductViews",
                column: "MenuItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductViews");
        }
    }
}
