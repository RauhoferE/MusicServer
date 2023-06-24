using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddedSortingOrderToFavoritesAndPlaylists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("182c6c95-08bf-44b1-9bfb-a322000e259c"));

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "PlaylistSongs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "FavoriteSongs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "5adae21b-b289-4623-a653-f0629cf9d1f0", new DateTime(2023, 6, 24, 22, 49, 13, 936, DateTimeKind.Local).AddTicks(8042), new DateTime(2023, 6, 24, 22, 49, 13, 936, DateTimeKind.Local).AddTicks(8053) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "1fb6c945-d414-4892-b457-b85da2c5034d", new DateTime(2023, 6, 24, 22, 49, 13, 936, DateTimeKind.Local).AddTicks(8111), new DateTime(2023, 6, 24, 22, 49, 13, 936, DateTimeKind.Local).AddTicks(8113) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("afad2cbd-29ce-466d-8648-8859115763c5"), new DateTime(2023, 6, 24, 22, 49, 13, 938, DateTimeKind.Local).AddTicks(574), null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("afad2cbd-29ce-466d-8648-8859115763c5"));

            migrationBuilder.DropColumn(
                name: "Order",
                table: "PlaylistSongs");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "FavoriteSongs");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "29562a83-3d81-4c04-aec5-2d1e0199e435", new DateTime(2023, 3, 6, 11, 48, 6, 381, DateTimeKind.Local).AddTicks(3112), new DateTime(2023, 3, 6, 11, 48, 6, 381, DateTimeKind.Local).AddTicks(3133) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "4af9ff03-a6a4-4821-ac22-3c65bcec8604", new DateTime(2023, 3, 6, 11, 48, 6, 381, DateTimeKind.Local).AddTicks(3214), new DateTime(2023, 3, 6, 11, 48, 6, 381, DateTimeKind.Local).AddTicks(3216) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("182c6c95-08bf-44b1-9bfb-a322000e259c"), new DateTime(2023, 3, 6, 11, 48, 6, 391, DateTimeKind.Local).AddTicks(3434), null, null });
        }
    }
}
