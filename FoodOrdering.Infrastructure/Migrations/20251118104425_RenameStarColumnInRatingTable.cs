using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrdering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameStarColumnInRatingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Starts",
                table: "Rating",
                newName: "Stars");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "Rating",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<double>(
                name: "AverageRating",
                table: "Menus",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_OrderId",
                table: "Rating",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Orders_OrderId",
                table: "Rating",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Orders_OrderId",
                table: "Rating");

            migrationBuilder.DropIndex(
                name: "IX_Rating_OrderId",
                table: "Rating");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Rating");

            migrationBuilder.RenameColumn(
                name: "Stars",
                table: "Rating",
                newName: "Starts");

            migrationBuilder.AlterColumn<float>(
                name: "AverageRating",
                table: "Menus",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
