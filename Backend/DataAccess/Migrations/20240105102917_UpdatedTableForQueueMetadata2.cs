using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class UpdatedTableForQueueMetadata2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueueData_LoopModes_LoopModeId",
                table: "QueueData");

            migrationBuilder.DropForeignKey(
                name: "FK_QueueData_LutQueueTargets_TargetId",
                table: "QueueData");

            migrationBuilder.DropTable(
                name: "LoopModes");

            migrationBuilder.DropTable(
                name: "LutQueueTargets");

            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("a9d1b2de-5c2c-4fe7-bfa6-51bab53c90ba"));

            migrationBuilder.AddColumn<int>(
                name: "SortAfterId",
                table: "QueueData",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LovLoopModes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LovLoopModes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LovPlaylistSortAfter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LovPlaylistSortAfter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LovQueueTargets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LovQueueTargets", x => x.Id);
                });

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

            migrationBuilder.InsertData(
                table: "LovLoopModes",
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
                table: "LovPlaylistSortAfter",
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
                table: "LovQueueTargets",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "favorites" },
                    { 2, "playlist" },
                    { 3, "album" },
                    { 4, "song" }
                });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("5c2a8e35-fc26-4205-ace1-327bbb18abc7"), new DateTime(2024, 1, 5, 11, 29, 16, 497, DateTimeKind.Local).AddTicks(4805), null, null });

            migrationBuilder.CreateIndex(
                name: "IX_QueueData_SortAfterId",
                table: "QueueData",
                column: "SortAfterId");

            migrationBuilder.AddForeignKey(
                name: "FK_QueueData_LovLoopModes_LoopModeId",
                table: "QueueData",
                column: "LoopModeId",
                principalTable: "LovLoopModes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QueueData_LovPlaylistSortAfter_SortAfterId",
                table: "QueueData",
                column: "SortAfterId",
                principalTable: "LovPlaylistSortAfter",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QueueData_LovQueueTargets_TargetId",
                table: "QueueData",
                column: "TargetId",
                principalTable: "LovQueueTargets",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueueData_LovLoopModes_LoopModeId",
                table: "QueueData");

            migrationBuilder.DropForeignKey(
                name: "FK_QueueData_LovPlaylistSortAfter_SortAfterId",
                table: "QueueData");

            migrationBuilder.DropForeignKey(
                name: "FK_QueueData_LovQueueTargets_TargetId",
                table: "QueueData");

            migrationBuilder.DropTable(
                name: "LovLoopModes");

            migrationBuilder.DropTable(
                name: "LovPlaylistSortAfter");

            migrationBuilder.DropTable(
                name: "LovQueueTargets");

            migrationBuilder.DropIndex(
                name: "IX_QueueData_SortAfterId",
                table: "QueueData");

            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("5c2a8e35-fc26-4205-ace1-327bbb18abc7"));

            migrationBuilder.DropColumn(
                name: "SortAfterId",
                table: "QueueData");

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
                values: new object[] { new Guid("a9d1b2de-5c2c-4fe7-bfa6-51bab53c90ba"), new DateTime(2024, 1, 5, 10, 39, 48, 220, DateTimeKind.Local).AddTicks(5954), null, null });

            migrationBuilder.AddForeignKey(
                name: "FK_QueueData_LoopModes_LoopModeId",
                table: "QueueData",
                column: "LoopModeId",
                principalTable: "LoopModes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QueueData_LutQueueTargets_TargetId",
                table: "QueueData",
                column: "TargetId",
                principalTable: "LutQueueTargets",
                principalColumn: "Id");
        }
    }
}
