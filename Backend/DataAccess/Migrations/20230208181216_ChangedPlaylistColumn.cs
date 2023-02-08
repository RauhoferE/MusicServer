using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class ChangedPlaylistColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReceiveNotifications",
                table: "PlaylistUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "5bdfa173-bccf-46f8-8357-b74bfe3e48a3", new DateTime(2023, 2, 8, 19, 12, 15, 742, DateTimeKind.Local).AddTicks(8984), new DateTime(2023, 2, 8, 19, 12, 15, 742, DateTimeKind.Local).AddTicks(9015) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "4ab66979-eb9e-4186-aa90-bbea8b5fb3f2", new DateTime(2023, 2, 8, 19, 12, 15, 742, DateTimeKind.Local).AddTicks(9074), new DateTime(2023, 2, 8, 19, 12, 15, 742, DateTimeKind.Local).AddTicks(9076) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "c4d471a9-bd4e-4449-a49a-2b9eca78850d", new DateTime(2023, 2, 8, 19, 12, 15, 742, DateTimeKind.Local).AddTicks(9082), new DateTime(2023, 2, 8, 19, 12, 15, 742, DateTimeKind.Local).AddTicks(9083) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiveNotifications",
                table: "PlaylistUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "765b8088-ebf7-4b8e-9264-dacb8ef4173f", new DateTime(2023, 1, 21, 9, 56, 33, 176, DateTimeKind.Local).AddTicks(6288), new DateTime(2023, 1, 21, 9, 56, 33, 176, DateTimeKind.Local).AddTicks(6323) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "ee6459c1-db05-420b-8dee-9e50fb157c9f", new DateTime(2023, 1, 21, 9, 56, 33, 176, DateTimeKind.Local).AddTicks(6399), new DateTime(2023, 1, 21, 9, 56, 33, 176, DateTimeKind.Local).AddTicks(6401) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "0c250db5-78c2-4155-bc11-ff5a3101b33c", new DateTime(2023, 1, 21, 9, 56, 33, 176, DateTimeKind.Local).AddTicks(6408), new DateTime(2023, 1, 21, 9, 56, 33, 176, DateTimeKind.Local).AddTicks(6409) });
        }
    }
}
