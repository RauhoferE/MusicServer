using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddedNewColumnInUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TemporarayEmail",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "6451952f-32c8-44ce-bd42-2bf11d5dc101", new DateTime(2023, 2, 26, 15, 9, 5, 626, DateTimeKind.Local).AddTicks(7408), new DateTime(2023, 2, 26, 15, 9, 5, 626, DateTimeKind.Local).AddTicks(7422) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "eb52d34f-5305-4140-87cc-060dfa03cef0", new DateTime(2023, 2, 26, 15, 9, 5, 626, DateTimeKind.Local).AddTicks(7480), new DateTime(2023, 2, 26, 15, 9, 5, 626, DateTimeKind.Local).AddTicks(7482) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "fdab3561-2d69-4760-a31c-f0b5e6712b3a", new DateTime(2023, 2, 26, 15, 9, 5, 626, DateTimeKind.Local).AddTicks(7488), new DateTime(2023, 2, 26, 15, 9, 5, 626, DateTimeKind.Local).AddTicks(7489) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemporarayEmail",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "6eb72c4b-d8c6-40fc-8395-cc1bda75bc34", new DateTime(2023, 2, 8, 20, 36, 41, 571, DateTimeKind.Local).AddTicks(5006), new DateTime(2023, 2, 8, 20, 36, 41, 571, DateTimeKind.Local).AddTicks(5017) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "a9200d6f-4726-4d28-a362-e8a1b80db78d", new DateTime(2023, 2, 8, 20, 36, 41, 571, DateTimeKind.Local).AddTicks(5077), new DateTime(2023, 2, 8, 20, 36, 41, 571, DateTimeKind.Local).AddTicks(5078) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "9f1e422a-3a32-4958-b6c7-8433e6eb4657", new DateTime(2023, 2, 8, 20, 36, 41, 571, DateTimeKind.Local).AddTicks(5097), new DateTime(2023, 2, 8, 20, 36, 41, 571, DateTimeKind.Local).AddTicks(5098) });
        }
    }
}
