using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddQueueEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("76e2df2c-b7ef-4b9b-9eeb-063716c3b543"));

            migrationBuilder.CreateTable(
                name: "Queues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SongId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Queues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Queues_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "affac61d-d25e-472a-8da1-c20fe5156e13", new DateTime(2023, 12, 27, 11, 49, 9, 475, DateTimeKind.Local).AddTicks(7053), new DateTime(2023, 12, 27, 11, 49, 9, 475, DateTimeKind.Local).AddTicks(7094) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "ca08b474-5f3f-4abc-a873-2dfb08b2b292", new DateTime(2023, 12, 27, 11, 49, 9, 475, DateTimeKind.Local).AddTicks(7238), new DateTime(2023, 12, 27, 11, 49, 9, 475, DateTimeKind.Local).AddTicks(7241) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2023, 12, 27, 11, 49, 9, 475, DateTimeKind.Local).AddTicks(7922), new DateTime(2023, 12, 27, 11, 49, 9, 475, DateTimeKind.Local).AddTicks(7930) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2023, 12, 27, 11, 49, 9, 475, DateTimeKind.Local).AddTicks(7980), new DateTime(2023, 12, 27, 11, 49, 9, 475, DateTimeKind.Local).AddTicks(7982) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2023, 12, 27, 11, 49, 9, 475, DateTimeKind.Local).AddTicks(7985), new DateTime(2023, 12, 27, 11, 49, 9, 475, DateTimeKind.Local).AddTicks(7987) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2023, 12, 27, 11, 49, 9, 475, DateTimeKind.Local).AddTicks(7990), new DateTime(2023, 12, 27, 11, 49, 9, 475, DateTimeKind.Local).AddTicks(7991) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2023, 12, 27, 11, 49, 9, 475, DateTimeKind.Local).AddTicks(7994), new DateTime(2023, 12, 27, 11, 49, 9, 475, DateTimeKind.Local).AddTicks(7995) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2023, 12, 27, 11, 49, 9, 475, DateTimeKind.Local).AddTicks(7998), new DateTime(2023, 12, 27, 11, 49, 9, 475, DateTimeKind.Local).AddTicks(7999) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("a0a122dd-1590-40f6-98d2-0907571fd1fc"), new DateTime(2023, 12, 27, 11, 49, 9, 477, DateTimeKind.Local).AddTicks(5493), null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Queues_SongId",
                table: "Queues",
                column: "SongId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Queues");

            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("a0a122dd-1590-40f6-98d2-0907571fd1fc"));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "253c28a8-28c2-45a1-bcd2-d379aeb8387d", new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7420), new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7430) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "f891a34b-1259-4fe4-883d-1fb3e065dbf5", new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7497), new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7499) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7784), new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7787) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7794), new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7796) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7799), new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7800) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7803), new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7804) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7807), new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7808) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7811), new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7812) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("76e2df2c-b7ef-4b9b-9eeb-063716c3b543"), new DateTime(2023, 7, 4, 20, 3, 2, 334, DateTimeKind.Local).AddTicks(9850), null, null });
        }
    }
}
