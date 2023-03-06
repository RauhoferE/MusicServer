using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddedSongFavorites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Songs_AspNetUsers_UserId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Songs_UserId",
                table: "Songs");

            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("3cc85e41-e0b3-4dcf-9cb6-8b143bf432d3"));

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Songs");

            migrationBuilder.CreateTable(
                name: "FavoriteSongs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    FavoriteSongId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteSongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteSongs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteSongs_Songs_FavoriteSongId",
                        column: x => x.FavoriteSongId,
                        principalTable: "Songs",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "29562a83-3d81-4c04-aec5-2d1e0199e435", new DateTime(2023, 3, 6, 11, 48, 6, 381, DateTimeKind.Local).AddTicks(3112), new DateTime(2023, 3, 6, 11, 48, 6, 381, DateTimeKind.Local).AddTicks(3133) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "4af9ff03-a6a4-4821-ac22-3c65bcec8604", new DateTime(2023, 3, 6, 11, 48, 6, 381, DateTimeKind.Local).AddTicks(3214), new DateTime(2023, 3, 6, 11, 48, 6, 381, DateTimeKind.Local).AddTicks(3216) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("182c6c95-08bf-44b1-9bfb-a322000e259c"), new DateTime(2023, 3, 6, 11, 48, 6, 391, DateTimeKind.Local).AddTicks(3434), null, null });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteSongs_FavoriteSongId",
                table: "FavoriteSongs",
                column: "FavoriteSongId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteSongs_UserId",
                table: "FavoriteSongs",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteSongs");

            migrationBuilder.DeleteData(
                table: "RegistrationCodes",
                keyColumn: "Id",
                keyValue: new Guid("182c6c95-08bf-44b1-9bfb-a322000e259c"));

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Songs",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "b42f80bf-7460-44ef-888e-151cf34e0429", new DateTime(2023, 3, 3, 15, 30, 23, 336, DateTimeKind.Local).AddTicks(40), new DateTime(2023, 3, 3, 15, 30, 23, 336, DateTimeKind.Local).AddTicks(51) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "e2b0126d-5664-463c-bfb6-7a18a35cc542", new DateTime(2023, 3, 3, 15, 30, 23, 336, DateTimeKind.Local).AddTicks(125), new DateTime(2023, 3, 3, 15, 30, 23, 336, DateTimeKind.Local).AddTicks(126) });

            migrationBuilder.InsertData(
                table: "RegistrationCodes",
                columns: new[] { "Id", "CreatedDate", "UsedByEmail", "UsedDate" },
                values: new object[] { new Guid("3cc85e41-e0b3-4dcf-9cb6-8b143bf432d3"), new DateTime(2023, 3, 3, 15, 30, 23, 337, DateTimeKind.Local).AddTicks(2673), null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Songs_UserId",
                table: "Songs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_AspNetUsers_UserId",
                table: "Songs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
