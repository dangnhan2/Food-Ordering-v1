using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrdering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmailOtpTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmailOtp_UserId",
                table: "EmailOtp");

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "EmailOtp",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_EmailOtp_UserId",
                table: "EmailOtp",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmailOtp_UserId",
                table: "EmailOtp");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "EmailOtp");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOtp_UserId",
                table: "EmailOtp",
                column: "UserId",
                unique: true);
        }
    }
}
