using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Triply.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicIdToHotelImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "HotelImages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "HotelImages");
        }
    }
}
