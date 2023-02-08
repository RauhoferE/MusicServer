using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddNewTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_AspNetUsers_UserId",
                table: "Artists");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_UserId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_Artists_UserId",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Artists");

            migrationBuilder.CreateTable(
                name: "UserArtists",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserArtists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserArtists_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserArtists_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FollowedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserUsers_AspNetUsers_FollowedUserId",
                        column: x => x.FollowedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "6eb72c4b-d8c6-40fc-8395-cc1bda75bc34", new DateTime(2023, 2, 8, 20, 36, 41, 571, DateTimeKind.Local).AddTicks(5006), new DateTime(2023, 2, 8, 20, 36, 41, 571, DateTimeKind.Local).AddTicks(5017) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "a9200d6f-4726-4d28-a362-e8a1b80db78d", new DateTime(2023, 2, 8, 20, 36, 41, 571, DateTimeKind.Local).AddTicks(5077), new DateTime(2023, 2, 8, 20, 36, 41, 571, DateTimeKind.Local).AddTicks(5078) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "9f1e422a-3a32-4958-b6c7-8433e6eb4657", new DateTime(2023, 2, 8, 20, 36, 41, 571, DateTimeKind.Local).AddTicks(5097), new DateTime(2023, 2, 8, 20, 36, 41, 571, DateTimeKind.Local).AddTicks(5098) });

            migrationBuilder.CreateIndex(
                name: "IX_UserArtists_ArtistId",
                table: "UserArtists",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_UserArtists_UserId",
                table: "UserArtists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUsers_FollowedUserId",
                table: "UserUsers",
                column: "FollowedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUsers_UserId",
                table: "UserUsers",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserArtists");

            migrationBuilder.DropTable(
                name: "UserUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Artists",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "5bdfa173-bccf-46f8-8357-b74bfe3e48a3", new DateTime(2023, 2, 8, 19, 12, 15, 742, DateTimeKind.Local).AddTicks(8984), new DateTime(2023, 2, 8, 19, 12, 15, 742, DateTimeKind.Local).AddTicks(9015) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "4ab66979-eb9e-4186-aa90-bbea8b5fb3f2", new DateTime(2023, 2, 8, 19, 12, 15, 742, DateTimeKind.Local).AddTicks(9074), new DateTime(2023, 2, 8, 19, 12, 15, 742, DateTimeKind.Local).AddTicks(9076) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "c4d471a9-bd4e-4449-a49a-2b9eca78850d", new DateTime(2023, 2, 8, 19, 12, 15, 742, DateTimeKind.Local).AddTicks(9082), new DateTime(2023, 2, 8, 19, 12, 15, 742, DateTimeKind.Local).AddTicks(9083) });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserId",
                table: "AspNetUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Artists_UserId",
                table: "Artists",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_AspNetUsers_UserId",
                table: "Artists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_UserId",
                table: "AspNetUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
