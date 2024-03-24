using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class UpdatedGroupEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("ba9b90b6-9a4a-48ce-a21e-f96747b70bdf"));

            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "80ac9e86-a93c-412e-b33d-592a806b9fd4", new DateTime(2024, 3, 24, 15, 22, 20, 824, DateTimeKind.Local).AddTicks(5009), new DateTime(2024, 3, 24, 15, 22, 20, 824, DateTimeKind.Local).AddTicks(5056) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "d09791a9-0519-4067-8123-6410a815d5a5", new DateTime(2024, 3, 24, 15, 22, 20, 824, DateTimeKind.Local).AddTicks(5164), new DateTime(2024, 3, 24, 15, 22, 20, 824, DateTimeKind.Local).AddTicks(5167) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 24, 15, 22, 20, 824, DateTimeKind.Local).AddTicks(5784), new DateTime(2024, 3, 24, 15, 22, 20, 824, DateTimeKind.Local).AddTicks(5793) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 24, 15, 22, 20, 824, DateTimeKind.Local).AddTicks(5807), new DateTime(2024, 3, 24, 15, 22, 20, 824, DateTimeKind.Local).AddTicks(5809) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 24, 15, 22, 20, 824, DateTimeKind.Local).AddTicks(5814), new DateTime(2024, 3, 24, 15, 22, 20, 824, DateTimeKind.Local).AddTicks(5815) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 24, 15, 22, 20, 824, DateTimeKind.Local).AddTicks(5819), new DateTime(2024, 3, 24, 15, 22, 20, 824, DateTimeKind.Local).AddTicks(5821) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 24, 15, 22, 20, 824, DateTimeKind.Local).AddTicks(5825), new DateTime(2024, 3, 24, 15, 22, 20, 824, DateTimeKind.Local).AddTicks(5827) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 24, 15, 22, 20, 824, DateTimeKind.Local).AddTicks(5832), new DateTime(2024, 3, 24, 15, 22, 20, 824, DateTimeKind.Local).AddTicks(5834) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("19525417-ce9d-4e54-9861-3a6ce6801483"), new DateTime(2024, 3, 24, 15, 22, 20, 827, DateTimeKind.Local).AddTicks(4962), null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("19525417-ce9d-4e54-9861-3a6ce6801483"));

            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "Groups");

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
    }
}
