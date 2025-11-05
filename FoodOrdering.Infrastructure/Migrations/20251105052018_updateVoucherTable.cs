using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FoodOrdering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateVoucherTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("89d000c1-2db0-4cc0-aab3-392d5519946f"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("fd8e246b-5197-4a72-9e3e-8c84e58466bc"));

            migrationBuilder.AlterColumn<int>(
                name: "MinOrderAmount",
                table: "Voucher",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaxDiscount",
                table: "Voucher",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DiscountValue",
                table: "Voucher",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("73c27aa4-d57b-4165-b896-f17d66015c41"), null, "Customer", "CUSTOMER" },
                    { new Guid("f758caa4-b53b-4c41-8e3c-5bcef3062f20"), null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("73c27aa4-d57b-4165-b896-f17d66015c41"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("f758caa4-b53b-4c41-8e3c-5bcef3062f20"));

            migrationBuilder.AlterColumn<decimal>(
                name: "MinOrderAmount",
                table: "Voucher",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxDiscount",
                table: "Voucher",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountValue",
                table: "Voucher",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("89d000c1-2db0-4cc0-aab3-392d5519946f"), null, "Customer", "CUSTOMER" },
                    { new Guid("fd8e246b-5197-4a72-9e3e-8c84e58466bc"), null, "Admin", "ADMIN" }
                });
        }
    }
}
