using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Triply.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelTypeAddressAndCityThumbnail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HotelAmenities_HotelId",
                table: "HotelAmenities");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Hotels",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HotelType",
                table: "Hotels",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Budget");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailPublicId",
                table: "Cities",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Cities",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_HotelType",
                table: "Hotels",
                column: "HotelType");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Hotels_HotelType",
                table: "Hotels",
                sql: "[HotelType] IN ('Budget', 'Boutique', 'Luxury')");

            migrationBuilder.CreateIndex(
                name: "UQ_HotelAmenities_Hotel_Amenity",
                table: "HotelAmenities",
                columns: new[] { "HotelId", "AmenityId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Hotels_HotelType",
                table: "Hotels");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Hotels_HotelType",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "UQ_HotelAmenities_Hotel_Amenity",
                table: "HotelAmenities");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "HotelType",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "ThumbnailPublicId",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Cities");

            migrationBuilder.CreateIndex(
                name: "IX_HotelAmenities_HotelId",
                table: "HotelAmenities",
                column: "HotelId");
        }
    }
}
