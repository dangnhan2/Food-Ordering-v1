using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FoodOrdering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixproperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("13e06384-021b-402b-8c12-c4607189b626"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("bff4375f-b678-493e-86c8-3a663ec0d769"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("600fe2bd-845a-442d-8d75-c216ef876b95"), null, "Admin", "ADMIN" },
                    { new Guid("d9a5e0be-3a3b-4aaa-afc6-047efa857780"), null, "Customer", "CUSTOMER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("600fe2bd-845a-442d-8d75-c216ef876b95"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("d9a5e0be-3a3b-4aaa-afc6-047efa857780"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("13e06384-021b-402b-8c12-c4607189b626"), null, "Customer", "CUSTOMER" },
                    { new Guid("bff4375f-b678-493e-86c8-3a663ec0d769"), null, "Admin", "ADMIN" }
                });
        }
    }
}
