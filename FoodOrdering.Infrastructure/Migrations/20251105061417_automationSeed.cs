using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FoodOrdering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class automationSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("73c27aa4-d57b-4165-b896-f17d66015c41"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("f758caa4-b53b-4c41-8e3c-5bcef3062f20"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("73c27aa4-d57b-4165-b896-f17d66015c41"), null, "Customer", "CUSTOMER" },
                    { new Guid("f758caa4-b53b-4c41-8e3c-5bcef3062f20"), null, "Admin", "ADMIN" }
                });
        }
    }
}
