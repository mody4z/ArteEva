using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ArtEva.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionMessage",
                table: "Shops",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, "32d082bf-f1d6-4ed4-a80e-8342488a2da8", new DateTime(2025, 12, 2, 23, 56, 45, 711, DateTimeKind.Utc).AddTicks(8405), "Administrator with full system access", "Admin", "ADMIN" },
                    { 2, "f7d716f7-68a6-4789-8513-8ae69c0f4f1e", new DateTime(2025, 12, 2, 23, 56, 45, 711, DateTimeKind.Utc).AddTicks(8422), "Customer who can browse and purchase artworks", "Buyer", "BUYER" },
                    { 3, "258ef476-1a42-4f68-9334-a89801af2847", new DateTime(2025, 12, 2, 23, 56, 45, 711, DateTimeKind.Utc).AddTicks(8431), "Artist who can create shop and sell artworks", "Seller", "SELLER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "RejectionMessage",
                table: "Shops");
        }
    }
}
