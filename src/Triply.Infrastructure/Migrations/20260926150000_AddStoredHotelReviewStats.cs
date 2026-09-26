using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Triply.Infrastructure.Db;

#nullable disable

namespace Triply.Infrastructure.Migrations;

/// <summary>Adds the denormalized guest-rating summary stored on each hotel.</summary>
[DbContext(typeof(TriplyDbContext))]
[Migration("20260926150000_AddStoredHotelReviewStats")]
public partial class AddStoredHotelReviewStats : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "AverageRating",
            table: "Hotels",
            type: "decimal(3,1)",
            precision: 3,
            scale: 1,
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ReviewsCount",
            table: "Hotels",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.Sql("""
            UPDATE hotel
            SET AverageRating = reviewStats.AverageRating,
                ReviewsCount = reviewStats.ReviewsCount
            FROM Hotels AS hotel
            OUTER APPLY
            (
                SELECT
                    CAST(ROUND(AVG(CAST(review.Rating AS decimal(5, 2))), 1) AS decimal(3, 1)) AS AverageRating,
                    COUNT(*) AS ReviewsCount
                FROM Reviews AS review
                WHERE review.HotelId = hotel.HotelId
            ) AS reviewStats;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "AverageRating", table: "Hotels");
        migrationBuilder.DropColumn(name: "ReviewsCount", table: "Hotels");
    }
}
