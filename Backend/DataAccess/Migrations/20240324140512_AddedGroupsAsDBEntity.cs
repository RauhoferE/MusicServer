using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddedGroupsAsDBEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("09450ae8-899b-4642-8070-f556ab7ccf6d"));

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsMaster = table.Column<bool>(type: "bit", nullable: false),
                    GroupName = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "a1365807-8bc3-4a76-8474-751ec4e02cbf", new DateTime(2024, 3, 24, 15, 5, 11, 507, DateTimeKind.Local).AddTicks(7339), new DateTime(2024, 3, 24, 15, 5, 11, 507, DateTimeKind.Local).AddTicks(7381) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "83f07292-5c69-4ae9-a876-6369a54a4ee3", new DateTime(2024, 3, 24, 15, 5, 11, 507, DateTimeKind.Local).AddTicks(7576), new DateTime(2024, 3, 24, 15, 5, 11, 507, DateTimeKind.Local).AddTicks(7579) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 24, 15, 5, 11, 507, DateTimeKind.Local).AddTicks(8440), new DateTime(2024, 3, 24, 15, 5, 11, 507, DateTimeKind.Local).AddTicks(8452) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 24, 15, 5, 11, 507, DateTimeKind.Local).AddTicks(8535), new DateTime(2024, 3, 24, 15, 5, 11, 507, DateTimeKind.Local).AddTicks(8537) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 24, 15, 5, 11, 507, DateTimeKind.Local).AddTicks(8542), new DateTime(2024, 3, 24, 15, 5, 11, 507, DateTimeKind.Local).AddTicks(8544) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 24, 15, 5, 11, 507, DateTimeKind.Local).AddTicks(8548), new DateTime(2024, 3, 24, 15, 5, 11, 507, DateTimeKind.Local).AddTicks(8550) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 24, 15, 5, 11, 507, DateTimeKind.Local).AddTicks(8553), new DateTime(2024, 3, 24, 15, 5, 11, 507, DateTimeKind.Local).AddTicks(8556) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 24, 15, 5, 11, 507, DateTimeKind.Local).AddTicks(8559), new DateTime(2024, 3, 24, 15, 5, 11, 507, DateTimeKind.Local).AddTicks(8562) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("ba9b90b6-9a4a-48ce-a21e-f96747b70bdf"), new DateTime(2024, 3, 24, 15, 5, 11, 511, DateTimeKind.Local).AddTicks(4142), null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("ba9b90b6-9a4a-48ce-a21e-f96747b70bdf"));

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
    }
}
