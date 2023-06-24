using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddedSortingOrderToPlaylists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("afad2cbd-29ce-466d-8648-8859115763c5"));

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "PlaylistUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "15bf5444-8bd7-45c8-8a42-fb6fedd0875b", new DateTime(2023, 6, 24, 23, 34, 23, 718, DateTimeKind.Local).AddTicks(9290), new DateTime(2023, 6, 24, 23, 34, 23, 718, DateTimeKind.Local).AddTicks(9301) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "30a96ad7-5d2a-4c14-99cf-4295d75c090a", new DateTime(2023, 6, 24, 23, 34, 23, 718, DateTimeKind.Local).AddTicks(9362), new DateTime(2023, 6, 24, 23, 34, 23, 718, DateTimeKind.Local).AddTicks(9363) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("8324ea8b-997f-4b8f-8feb-eb523bb1aa78"), new DateTime(2023, 6, 24, 23, 34, 23, 720, DateTimeKind.Local).AddTicks(1645), null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("8324ea8b-997f-4b8f-8feb-eb523bb1aa78"));

            migrationBuilder.DropColumn(
                name: "Order",
                table: "PlaylistUsers");

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
    }
}
