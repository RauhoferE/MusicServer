using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class UpdatedTableForQueueMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("1d36991e-9fcf-4b53-b468-deb1988fb897"));

            migrationBuilder.AddColumn<int>(
                name: "LoopModeId",
                table: "QueueData",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Random",
                table: "QueueData",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "LoopModes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoopModes", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "9bdfadfd-e741-4828-8502-5350f51fd489", new DateTime(2024, 1, 5, 10, 39, 48, 218, DateTimeKind.Local).AddTicks(8302), new DateTime(2024, 1, 5, 10, 39, 48, 218, DateTimeKind.Local).AddTicks(8345) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "683c2f0f-56aa-4040-bd68-b5054daff45b", new DateTime(2024, 1, 5, 10, 39, 48, 218, DateTimeKind.Local).AddTicks(8503), new DateTime(2024, 1, 5, 10, 39, 48, 218, DateTimeKind.Local).AddTicks(8506) });

            migrationBuilder.InsertData(
                table: "LoopModes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "none" },
                    { 2, "playlist" },
                    { 3, "audio" }
                });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 10, 39, 48, 218, DateTimeKind.Local).AddTicks(8999), new DateTime(2024, 1, 5, 10, 39, 48, 218, DateTimeKind.Local).AddTicks(9006) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 10, 39, 48, 218, DateTimeKind.Local).AddTicks(9053), new DateTime(2024, 1, 5, 10, 39, 48, 218, DateTimeKind.Local).AddTicks(9056) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 10, 39, 48, 218, DateTimeKind.Local).AddTicks(9060), new DateTime(2024, 1, 5, 10, 39, 48, 218, DateTimeKind.Local).AddTicks(9061) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 10, 39, 48, 218, DateTimeKind.Local).AddTicks(9064), new DateTime(2024, 1, 5, 10, 39, 48, 218, DateTimeKind.Local).AddTicks(9065) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 10, 39, 48, 218, DateTimeKind.Local).AddTicks(9068), new DateTime(2024, 1, 5, 10, 39, 48, 218, DateTimeKind.Local).AddTicks(9070) });

            migrationBuilder.UpdateData(
                table: "LovMessageTypes",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 1, 5, 10, 39, 48, 218, DateTimeKind.Local).AddTicks(9072), new DateTime(2024, 1, 5, 10, 39, 48, 218, DateTimeKind.Local).AddTicks(9074) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("a9d1b2de-5c2c-4fe7-bfa6-51bab53c90ba"), new DateTime(2024, 1, 5, 10, 39, 48, 220, DateTimeKind.Local).AddTicks(5954), null, null });

            migrationBuilder.CreateIndex(
                name: "IX_QueueData_LoopModeId",
                table: "QueueData",
                column: "LoopModeId");

            migrationBuilder.AddForeignKey(
                name: "FK_QueueData_LoopModes_LoopModeId",
                table: "QueueData",
                column: "LoopModeId",
                principalTable: "LoopModes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueueData_LoopModes_LoopModeId",
                table: "QueueData");

            migrationBuilder.DropTable(
                name: "LoopModes");

            migrationBuilder.DropIndex(
                name: "IX_QueueData_LoopModeId",
                table: "QueueData");

            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("a9d1b2de-5c2c-4fe7-bfa6-51bab53c90ba"));

            migrationBuilder.DropColumn(
                name: "LoopModeId",
                table: "QueueData");

            migrationBuilder.DropColumn(
                name: "Random",
                table: "QueueData");

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
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("1d36991e-9fcf-4b53-b468-deb1988fb897"), new DateTime(2024, 1, 5, 10, 20, 38, 262, DateTimeKind.Local).AddTicks(3152), null, null });
        }
    }
}
