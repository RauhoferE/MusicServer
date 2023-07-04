using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddedMessageQueue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("e4badfeb-32f8-4bba-a3f0-a3e0e7008697"));

            migrationBuilder.CreateTable(
                name: "LovMessageTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LovMessageTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageQueue",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TargetUserId = table.Column<long>(type: "bigint", nullable: false),
                    ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlaylistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageQueue_LovMessageTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "LovMessageTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MessageSongIds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<long>(type: "bigint", nullable: true),
                    SongId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageSongIds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageSongIds_MessageQueue_MessageId",
                        column: x => x.MessageId,
                        principalTable: "MessageQueue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.InsertData(
                table: "LovMessageTypes",
                columns: new[] { "Id", "Created", "Modified", "Name" },
                values: new object[,]
                {
                    { 1L, new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7784), new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7787), "PlaylistAdded" },
                    { 2L, new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7794), new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7796), "PlaylistSongsAdded" },
                    { 3L, new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7799), new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7800), "PlaylistShared" },
                    { 4L, new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7803), new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7804), "PlaylistShareRemoved" },
                    { 5L, new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7807), new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7808), "ArtistTracksAdded" },
                    { 6L, new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7811), new DateTime(2023, 7, 4, 20, 3, 2, 333, DateTimeKind.Local).AddTicks(7812), "ArtistAdded" }
                });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("76e2df2c-b7ef-4b9b-9eeb-063716c3b543"), new DateTime(2023, 7, 4, 20, 3, 2, 334, DateTimeKind.Local).AddTicks(9850), null, null });

            migrationBuilder.CreateIndex(
                name: "IX_MessageQueue_TypeId",
                table: "MessageQueue",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageSongIds_MessageId",
                table: "MessageSongIds",
                column: "MessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageSongIds");

            migrationBuilder.DropTable(
                name: "MessageQueue");

            migrationBuilder.DropTable(
                name: "LovMessageTypes");

            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("76e2df2c-b7ef-4b9b-9eeb-063716c3b543"));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "404e5774-75d3-4622-9067-6b93b2313121", new DateTime(2023, 6, 30, 18, 24, 53, 667, DateTimeKind.Local).AddTicks(3597), new DateTime(2023, 6, 30, 18, 24, 53, 667, DateTimeKind.Local).AddTicks(3608) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "73529c69-6d4b-4078-8f28-f4f48111f04a", new DateTime(2023, 6, 30, 18, 24, 53, 667, DateTimeKind.Local).AddTicks(3704), new DateTime(2023, 6, 30, 18, 24, 53, 667, DateTimeKind.Local).AddTicks(3706) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("e4badfeb-32f8-4bba-a3f0-a3e0e7008697"), new DateTime(2023, 6, 30, 18, 24, 53, 668, DateTimeKind.Local).AddTicks(6147), null, null });
        }
    }
}
