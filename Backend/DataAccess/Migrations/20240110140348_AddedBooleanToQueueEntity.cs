using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddedBooleanToQueueEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("5c2a8e35-fc26-4205-ace1-327bbb18abc7"));

            migrationBuilder.AddColumn<bool>(
                name: "AddedManualy",
                table: "Queues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "8773e20b-c962-42bd-bee4-67ef55b8d27a", new DateTime(2024, 1, 10, 15, 3, 47, 606, DateTimeKind.Local).AddTicks(877), new DateTime(2024, 1, 10, 15, 3, 47, 606, DateTimeKind.Local).AddTicks(911) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "ea8c2d86-139d-444d-9a3d-339f2ae08a4d", new DateTime(2024, 1, 10, 15, 3, 47, 606, DateTimeKind.Local).AddTicks(1047), new DateTime(2024, 1, 10, 15, 3, 47, 606, DateTimeKind.Local).AddTicks(1049) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 10, 15, 3, 47, 606, DateTimeKind.Local).AddTicks(1504), new DateTime(2024, 1, 10, 15, 3, 47, 606, DateTimeKind.Local).AddTicks(1511) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 10, 15, 3, 47, 606, DateTimeKind.Local).AddTicks(1520), new DateTime(2024, 1, 10, 15, 3, 47, 606, DateTimeKind.Local).AddTicks(1522) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 10, 15, 3, 47, 606, DateTimeKind.Local).AddTicks(1525), new DateTime(2024, 1, 10, 15, 3, 47, 606, DateTimeKind.Local).AddTicks(1527) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 10, 15, 3, 47, 606, DateTimeKind.Local).AddTicks(1529), new DateTime(2024, 1, 10, 15, 3, 47, 606, DateTimeKind.Local).AddTicks(1531) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 10, 15, 3, 47, 606, DateTimeKind.Local).AddTicks(1534), new DateTime(2024, 1, 10, 15, 3, 47, 606, DateTimeKind.Local).AddTicks(1535) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 10, 15, 3, 47, 606, DateTimeKind.Local).AddTicks(1538), new DateTime(2024, 1, 10, 15, 3, 47, 606, DateTimeKind.Local).AddTicks(1540) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("09450ae8-899b-4642-8070-f556ab7ccf6d"), new DateTime(2024, 1, 10, 15, 3, 47, 608, DateTimeKind.Local).AddTicks(388), null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("09450ae8-899b-4642-8070-f556ab7ccf6d"));

            migrationBuilder.DropColumn(
                name: "AddedManualy",
                table: "Queues");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "44a310d4-13a2-4d87-b764-667dd5d4c81e", new DateTime(2024, 1, 5, 11, 29, 16, 495, DateTimeKind.Local).AddTicks(6200), new DateTime(2024, 1, 5, 11, 29, 16, 495, DateTimeKind.Local).AddTicks(6236) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "de9b3ca4-9a0d-4e16-941f-0deca267bf22", new DateTime(2024, 1, 5, 11, 29, 16, 495, DateTimeKind.Local).AddTicks(6389), new DateTime(2024, 1, 5, 11, 29, 16, 495, DateTimeKind.Local).AddTicks(6392) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 11, 29, 16, 495, DateTimeKind.Local).AddTicks(6971), new DateTime(2024, 1, 5, 11, 29, 16, 495, DateTimeKind.Local).AddTicks(6978) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 11, 29, 16, 495, DateTimeKind.Local).AddTicks(7025), new DateTime(2024, 1, 5, 11, 29, 16, 495, DateTimeKind.Local).AddTicks(7027) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 11, 29, 16, 495, DateTimeKind.Local).AddTicks(7030), new DateTime(2024, 1, 5, 11, 29, 16, 495, DateTimeKind.Local).AddTicks(7031) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 11, 29, 16, 495, DateTimeKind.Local).AddTicks(7034), new DateTime(2024, 1, 5, 11, 29, 16, 495, DateTimeKind.Local).AddTicks(7035) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 11, 29, 16, 495, DateTimeKind.Local).AddTicks(7038), new DateTime(2024, 1, 5, 11, 29, 16, 495, DateTimeKind.Local).AddTicks(7040) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 11, 29, 16, 495, DateTimeKind.Local).AddTicks(7042), new DateTime(2024, 1, 5, 11, 29, 16, 495, DateTimeKind.Local).AddTicks(7044) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("5c2a8e35-fc26-4205-ace1-327bbb18abc7"), new DateTime(2024, 1, 5, 11, 29, 16, 497, DateTimeKind.Local).AddTicks(4805), null, null });
        }
    }
}
