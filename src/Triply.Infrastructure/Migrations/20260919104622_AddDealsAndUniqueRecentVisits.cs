using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Triply.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDealsAndUniqueRecentVisits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Deals",
                columns: table => new
                {
                    DealId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    StartsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deals", x => x.DealId);
                    table.CheckConstraint("CK_Deals_Date", "[EndsAt] > [StartsAt]");
                    table.CheckConstraint("CK_Deals_Discount", "[DiscountPercentage] > 0 AND [DiscountPercentage] <= 90");
                    table.ForeignKey(
                        name: "FK_Deals_Rooms",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_UserRecentVisits_User_Hotel",
                table: "UserRecentVisits",
                columns: new[] { "UserId", "HotelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deals_IsFeatured_EndsAt",
                table: "Deals",
                columns: new[] { "IsFeatured", "EndsAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Deals_RoomId_Dates",
                table: "Deals",
                columns: new[] { "RoomId", "StartsAt", "EndsAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deals");

            migrationBuilder.DropIndex(
                name: "UQ_UserRecentVisits_User_Hotel",
                table: "UserRecentVisits");
        }
    }
}
