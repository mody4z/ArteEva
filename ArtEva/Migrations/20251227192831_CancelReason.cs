using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtEva.Migrations
{
    /// <inheritdoc />
    public partial class CancelReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CancelledByUserId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "80df881c-d6b1-4715-b145-4c0e08d271d4", new DateTime(2025, 12, 27, 19, 28, 27, 460, DateTimeKind.Utc).AddTicks(9570) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "0ffb984e-3a1f-4e79-9e4d-5b1aada24577", new DateTime(2025, 12, 27, 19, 28, 27, 460, DateTimeKind.Utc).AddTicks(9581) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "4e106d6a-fc5e-4024-bf39-2c56c9df6776", new DateTime(2025, 12, 27, 19, 28, 27, 460, DateTimeKind.Utc).AddTicks(9589) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CancelledByUserId",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "60d5d71f-84b6-43b0-808f-6cc0cf274ee0", new DateTime(2025, 12, 25, 21, 20, 12, 824, DateTimeKind.Utc).AddTicks(6867) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "99c181e0-e76a-4d5e-a4e5-082abff0b1e3", new DateTime(2025, 12, 25, 21, 20, 12, 824, DateTimeKind.Utc).AddTicks(6878) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "73da00de-b936-4719-95b0-a9211f80d928", new DateTime(2025, 12, 25, 21, 20, 12, 824, DateTimeKind.Utc).AddTicks(6886) });
        }
    }
}
