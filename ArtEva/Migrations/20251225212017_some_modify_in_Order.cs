using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtEva.Migrations
{
    /// <inheritdoc />
    public partial class some_modify_in_Order : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "price",
                table: "CartItems",
                newName: "TotalPrice");

            migrationBuilder.AlterColumn<string>(
                name: "ProductImageSnapshot",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CartItemId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckedOutAt",
                table: "Carts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConvertedToOrder",
                table: "CartItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "CartItems",
                type: "int",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CartItemId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CheckedOutAt",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "IsConvertedToOrder",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "CartItems",
                newName: "price");

            migrationBuilder.AlterColumn<string>(
                name: "ProductImageSnapshot",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
    }
}
