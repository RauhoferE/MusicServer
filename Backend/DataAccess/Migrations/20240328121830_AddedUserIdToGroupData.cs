using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddedUserIdToGroupData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("f1901c0c-d7b2-4ef0-b841-ee33b8465ca5"));

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "GroupQueueData",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "5e94b4ad-9baf-4dc6-bcb3-d40c12cc4140", new DateTime(2024, 3, 28, 13, 18, 29, 872, DateTimeKind.Local).AddTicks(864), new DateTime(2024, 3, 28, 13, 18, 29, 872, DateTimeKind.Local).AddTicks(988) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "89e07e39-8a47-4a46-a329-1b08425d4e8b", new DateTime(2024, 3, 28, 13, 18, 29, 872, DateTimeKind.Local).AddTicks(1698), new DateTime(2024, 3, 28, 13, 18, 29, 872, DateTimeKind.Local).AddTicks(1723) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 28, 13, 18, 29, 872, DateTimeKind.Local).AddTicks(4629), new DateTime(2024, 3, 28, 13, 18, 29, 872, DateTimeKind.Local).AddTicks(4657) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 28, 13, 18, 29, 872, DateTimeKind.Local).AddTicks(4821), new DateTime(2024, 3, 28, 13, 18, 29, 872, DateTimeKind.Local).AddTicks(4825) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 28, 13, 18, 29, 872, DateTimeKind.Local).AddTicks(4830), new DateTime(2024, 3, 28, 13, 18, 29, 872, DateTimeKind.Local).AddTicks(4832) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 28, 13, 18, 29, 872, DateTimeKind.Local).AddTicks(4836), new DateTime(2024, 3, 28, 13, 18, 29, 872, DateTimeKind.Local).AddTicks(4838) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 28, 13, 18, 29, 872, DateTimeKind.Local).AddTicks(4842), new DateTime(2024, 3, 28, 13, 18, 29, 872, DateTimeKind.Local).AddTicks(4845) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 28, 13, 18, 29, 872, DateTimeKind.Local).AddTicks(4849), new DateTime(2024, 3, 28, 13, 18, 29, 872, DateTimeKind.Local).AddTicks(4851) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("c64bc29a-4963-467e-9d91-33732bb1bb84"), new DateTime(2024, 3, 28, 13, 18, 29, 876, DateTimeKind.Local).AddTicks(5355), null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("c64bc29a-4963-467e-9d91-33732bb1bb84"));

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "GroupQueueData");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "25996457-e16a-405b-8017-92b5ef66ebc0", new DateTime(2024, 3, 27, 12, 13, 4, 394, DateTimeKind.Local).AddTicks(8739), new DateTime(2024, 3, 27, 12, 13, 4, 394, DateTimeKind.Local).AddTicks(8783) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "8ca8af96-64b8-428b-bf9e-9c69a9673493", new DateTime(2024, 3, 27, 12, 13, 4, 394, DateTimeKind.Local).AddTicks(8891), new DateTime(2024, 3, 27, 12, 13, 4, 394, DateTimeKind.Local).AddTicks(8893) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 27, 12, 13, 4, 394, DateTimeKind.Local).AddTicks(9553), new DateTime(2024, 3, 27, 12, 13, 4, 394, DateTimeKind.Local).AddTicks(9562) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 27, 12, 13, 4, 394, DateTimeKind.Local).AddTicks(9572), new DateTime(2024, 3, 27, 12, 13, 4, 394, DateTimeKind.Local).AddTicks(9575) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 27, 12, 13, 4, 394, DateTimeKind.Local).AddTicks(9578), new DateTime(2024, 3, 27, 12, 13, 4, 394, DateTimeKind.Local).AddTicks(9579) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 27, 12, 13, 4, 394, DateTimeKind.Local).AddTicks(9582), new DateTime(2024, 3, 27, 12, 13, 4, 394, DateTimeKind.Local).AddTicks(9583) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 27, 12, 13, 4, 394, DateTimeKind.Local).AddTicks(9586), new DateTime(2024, 3, 27, 12, 13, 4, 394, DateTimeKind.Local).AddTicks(9588) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 27, 12, 13, 4, 394, DateTimeKind.Local).AddTicks(9591), new DateTime(2024, 3, 27, 12, 13, 4, 394, DateTimeKind.Local).AddTicks(9593) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("f1901c0c-d7b2-4ef0-b841-ee33b8465ca5"), new DateTime(2024, 3, 27, 12, 13, 4, 399, DateTimeKind.Local).AddTicks(464), null, null });
        }
    }
}
