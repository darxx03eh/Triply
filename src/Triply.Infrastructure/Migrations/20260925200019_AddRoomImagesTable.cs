using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Triply.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomImagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomImages",
                columns: table => new
                {
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PublicId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DisplayOrder = table.Column<short>(type: "smallint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomImages", x => x.ImageId);
                    table.CheckConstraint("CK_DisplayOrder_Positive1", "[DisplayOrder] > 0");
                    table.CheckConstraint("CK_Images_Status1", "[Status] IN ('Pending', 'Uploaded', 'Failed')");
                    table.ForeignKey(
                        name: "FK_RoomImages_Rooms",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomImages_RoomId",
                table: "RoomImages",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomImages");
        }
    }
}
