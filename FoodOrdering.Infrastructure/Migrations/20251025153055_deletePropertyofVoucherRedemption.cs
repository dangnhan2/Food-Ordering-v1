using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FoodOrdering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deletePropertyofVoucherRedemption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("793ca9d0-0762-4ab2-bb1f-8a13d3c8dbbf"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("8f5d19a9-5df4-4a5f-bb41-2962365221fb"));

            migrationBuilder.DropColumn(
                name: "AmountDiscount",
                table: "VoucherRedemptions");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "Menus");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiredAt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Addresses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("89d000c1-2db0-4cc0-aab3-392d5519946f"), null, "Customer", "CUSTOMER" },
                    { new Guid("fd8e246b-5197-4a72-9e3e-8c84e58466bc"), null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("89d000c1-2db0-4cc0-aab3-392d5519946f"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("fd8e246b-5197-4a72-9e3e-8c84e58466bc"));

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Addresses");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountDiscount",
                table: "VoucherRedemptions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiredAt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "Menus",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("793ca9d0-0762-4ab2-bb1f-8a13d3c8dbbf"), null, "Admin", "ADMIN" },
                    { new Guid("8f5d19a9-5df4-4a5f-bb41-2962365221fb"), null, "Customer", "CUSTOMER" }
                });
        }
    }
}
