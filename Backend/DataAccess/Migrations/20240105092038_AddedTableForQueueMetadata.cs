using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddedTableForQueueMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("a0a122dd-1590-40f6-98d2-0907571fd1fc"));

            migrationBuilder.CreateTable(
                name: "LutQueueTargets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LutQueueTargets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QueueData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Asc = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TargetId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueueData_LutQueueTargets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "LutQueueTargets",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "794eda8f-84a6-4ac6-ae5c-6f5897d9799a", new DateTime(2024, 1, 5, 10, 20, 38, 259, DateTimeKind.Local).AddTicks(4360), new DateTime(2024, 1, 5, 10, 20, 38, 259, DateTimeKind.Local).AddTicks(4397) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "432a442e-48b2-4194-a4df-f1db27245a3e", new DateTime(2024, 1, 5, 10, 20, 38, 259, DateTimeKind.Local).AddTicks(4539), new DateTime(2024, 1, 5, 10, 20, 38, 259, DateTimeKind.Local).AddTicks(4542) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 10, 20, 38, 259, DateTimeKind.Local).AddTicks(5231), new DateTime(2024, 1, 5, 10, 20, 38, 259, DateTimeKind.Local).AddTicks(5240) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 10, 20, 38, 259, DateTimeKind.Local).AddTicks(5286), new DateTime(2024, 1, 5, 10, 20, 38, 259, DateTimeKind.Local).AddTicks(5288) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 10, 20, 38, 259, DateTimeKind.Local).AddTicks(5291), new DateTime(2024, 1, 5, 10, 20, 38, 259, DateTimeKind.Local).AddTicks(5293) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 10, 20, 38, 259, DateTimeKind.Local).AddTicks(5295), new DateTime(2024, 1, 5, 10, 20, 38, 259, DateTimeKind.Local).AddTicks(5297) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 10, 20, 38, 259, DateTimeKind.Local).AddTicks(5299), new DateTime(2024, 1, 5, 10, 20, 38, 259, DateTimeKind.Local).AddTicks(5301) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 10, 20, 38, 259, DateTimeKind.Local).AddTicks(5303), new DateTime(2024, 1, 5, 10, 20, 38, 259, DateTimeKind.Local).AddTicks(5305) });

            migrationBuilder.InsertData(
                table: "LutQueueTargets",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "album" },
                    { 2, "name" },
                    { 3, "created" },
                    { 4, "artist" },
                    { 5, "duration" },
                    { 6, "order" }
                });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("1d36991e-9fcf-4b53-b468-deb1988fb897"), new DateTime(2024, 1, 5, 10, 20, 38, 262, DateTimeKind.Local).AddTicks(3152), null, null });

            migrationBuilder.CreateIndex(
                name: "IX_QueueData_TargetId",
                table: "QueueData",
                column: "TargetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QueueData");

            migrationBuilder.DropTable(
                name: "LutQueueTargets");

            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("1d36991e-9fcf-4b53-b468-deb1988fb897"));

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
        }
    }
}
