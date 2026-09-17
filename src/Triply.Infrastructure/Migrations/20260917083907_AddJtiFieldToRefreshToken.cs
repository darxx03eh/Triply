using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Triply.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJtiFieldToRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Jti",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Jti",
                table: "RefreshTokens");
        }
    }
}
