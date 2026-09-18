using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Triply.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToImagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "HotelImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Images_Status",
                table: "HotelImages",
                sql: "[Status] IN ('Pending', 'Uploaded', 'Failed')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Images_Status",
                table: "HotelImages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "HotelImages");
        }
    }
}
