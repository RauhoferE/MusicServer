using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddedNewDbSetsForGroupQueueData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("c0871e92-d495-4f54-a546-9783ead558b5"));

            migrationBuilder.CreateTable(
                name: "GroupQueueData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Asc = table.Column<bool>(type: "bit", nullable: false),
                    Random = table.Column<bool>(type: "bit", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortAfterId = table.Column<int>(type: "int", nullable: true),
                    TargetId = table.Column<int>(type: "int", nullable: true),
                    LoopModeId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupQueueData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupQueueData_LovLoopModes_LoopModeId",
                        column: x => x.LoopModeId,
                        principalTable: "LovLoopModes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GroupQueueData_LovPlaylistSortAfter_SortAfterId",
                        column: x => x.SortAfterId,
                        principalTable: "LovPlaylistSortAfter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GroupQueueData_LovQueueTargets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "LovQueueTargets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GroupQueueEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SongId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AddedManualy = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupQueueEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupQueueEntities_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_GroupQueueData_LoopModeId",
                table: "GroupQueueData",
                column: "LoopModeId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupQueueData_SortAfterId",
                table: "GroupQueueData",
                column: "SortAfterId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupQueueData_TargetId",
                table: "GroupQueueData",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupQueueEntities_SongId",
                table: "GroupQueueEntities",
                column: "SongId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupQueueData");

            migrationBuilder.DropTable(
                name: "GroupQueueEntities");

            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("f1901c0c-d7b2-4ef0-b841-ee33b8465ca5"));

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
    }
}
