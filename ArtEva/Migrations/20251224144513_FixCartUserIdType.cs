using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtEva.Migrations
{
    /// <inheritdoc />
    public partial class FixCartUserIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "71f25a02-d759-474b-bd09-14612d1f9056", new DateTime(2025, 12, 24, 14, 45, 8, 965, DateTimeKind.Utc).AddTicks(2161) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "136f080f-b538-43ef-9727-777ece92534d", new DateTime(2025, 12, 24, 14, 45, 8, 965, DateTimeKind.Utc).AddTicks(2178) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "b11904bb-1251-41c6-a838-a6990474e86e", new DateTime(2025, 12, 24, 14, 45, 8, 965, DateTimeKind.Utc).AddTicks(2186) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "baa4a69c-a9d0-4bf5-808c-b0fb2999c8d5", new DateTime(2025, 12, 24, 8, 5, 23, 585, DateTimeKind.Utc).AddTicks(5268) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "a84af176-cd6f-473a-829c-72cc3b9fc5f7", new DateTime(2025, 12, 24, 8, 5, 23, 585, DateTimeKind.Utc).AddTicks(5283) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "6ef1e5de-84a8-4218-817d-dbcd7281d08f", new DateTime(2025, 12, 24, 8, 5, 23, 585, DateTimeKind.Utc).AddTicks(5290) });
        }
    }
}
