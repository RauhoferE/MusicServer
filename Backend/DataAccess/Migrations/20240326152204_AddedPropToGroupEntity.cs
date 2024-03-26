using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddedPropToGroupEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("19525417-ce9d-4e54-9861-3a6ce6801483"));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "4a2579a7-46e6-4c44-bd07-4c86cc6acd06", new DateTime(2024, 3, 26, 16, 22, 4, 102, DateTimeKind.Local).AddTicks(9640), new DateTime(2024, 3, 26, 16, 22, 4, 102, DateTimeKind.Local).AddTicks(9678) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "357f6087-f1dc-455a-8115-70390e185b28", new DateTime(2024, 3, 26, 16, 22, 4, 102, DateTimeKind.Local).AddTicks(9761), new DateTime(2024, 3, 26, 16, 22, 4, 102, DateTimeKind.Local).AddTicks(9764) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 26, 16, 22, 4, 103, DateTimeKind.Local).AddTicks(147), new DateTime(2024, 3, 26, 16, 22, 4, 103, DateTimeKind.Local).AddTicks(153) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 26, 16, 22, 4, 103, DateTimeKind.Local).AddTicks(162), new DateTime(2024, 3, 26, 16, 22, 4, 103, DateTimeKind.Local).AddTicks(164) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 26, 16, 22, 4, 103, DateTimeKind.Local).AddTicks(167), new DateTime(2024, 3, 26, 16, 22, 4, 103, DateTimeKind.Local).AddTicks(168) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 26, 16, 22, 4, 103, DateTimeKind.Local).AddTicks(171), new DateTime(2024, 3, 26, 16, 22, 4, 103, DateTimeKind.Local).AddTicks(172) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 26, 16, 22, 4, 103, DateTimeKind.Local).AddTicks(175), new DateTime(2024, 3, 26, 16, 22, 4, 103, DateTimeKind.Local).AddTicks(177) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 26, 16, 22, 4, 103, DateTimeKind.Local).AddTicks(180), new DateTime(2024, 3, 26, 16, 22, 4, 103, DateTimeKind.Local).AddTicks(181) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("c0871e92-d495-4f54-a546-9783ead558b5"), new DateTime(2024, 3, 26, 16, 22, 4, 104, DateTimeKind.Local).AddTicks(7019), null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("c0871e92-d495-4f54-a546-9783ead558b5"));

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Groups");

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
    }
}
