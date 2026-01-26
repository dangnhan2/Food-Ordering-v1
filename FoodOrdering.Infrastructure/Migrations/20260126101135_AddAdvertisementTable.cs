using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrdering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvertisementTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Advertisement");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "EndAt",
                table: "Advertisement",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "EndAt",
                table: "Advertisement",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Advertisement",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
