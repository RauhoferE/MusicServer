using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class ChangedSomeProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("8324ea8b-997f-4b8f-8feb-eb523bb1aa78"));

            migrationBuilder.DropColumn(
                name: "LastListened",
                table: "Playlists");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastListened",
                table: "PlaylistUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReceiveNotifications",
                table: "FollowedUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReceiveNotifications",
                table: "FollowedArtists",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "404e5774-75d3-4622-9067-6b93b2313121", new DateTime(2023, 6, 30, 18, 24, 53, 667, DateTimeKind.Local).AddTicks(3597), new DateTime(2023, 6, 30, 18, 24, 53, 667, DateTimeKind.Local).AddTicks(3608) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "73529c69-6d4b-4078-8f28-f4f48111f04a", new DateTime(2023, 6, 30, 18, 24, 53, 667, DateTimeKind.Local).AddTicks(3704), new DateTime(2023, 6, 30, 18, 24, 53, 667, DateTimeKind.Local).AddTicks(3706) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("e4badfeb-32f8-4bba-a3f0-a3e0e7008697"), new DateTime(2023, 6, 30, 18, 24, 53, 668, DateTimeKind.Local).AddTicks(6147), null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("e4badfeb-32f8-4bba-a3f0-a3e0e7008697"));

            migrationBuilder.DropColumn(
                name: "LastListened",
                table: "PlaylistUsers");

            migrationBuilder.DropColumn(
                name: "ReceiveNotifications",
                table: "FollowedUsers");

            migrationBuilder.DropColumn(
                name: "ReceiveNotifications",
                table: "FollowedArtists");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastListened",
                table: "Playlists",
                type: "datetime2",
                nullable: true);

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
    }
}
