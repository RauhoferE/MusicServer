using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class ChangedPlaylistUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtistAlbums_Albums_AlbumId",
                table: "ArtistAlbums");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistAlbums_Artists_ArtistId",
                table: "ArtistAlbums");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistSongs_Artists_ArtistId",
                table: "ArtistSongs");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistSongs_Songs_SongId",
                table: "ArtistSongs");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistSongs_Playlists_PlaylistId",
                table: "PlaylistSongs");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistSongs_Songs_SongId",
                table: "PlaylistSongs");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistUsers_AspNetUsers_UserId",
                table: "PlaylistUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistUsers_Playlists_PlaylistId",
                table: "PlaylistUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Albums_AlbumId",
                table: "Songs");

            migrationBuilder.AddColumn<bool>(
                name: "IsCreator",
                table: "PlaylistUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "f1877f0f-d414-468e-aeae-dfa1d6ab303b", new DateTime(2023, 1, 7, 18, 46, 38, 881, DateTimeKind.Local).AddTicks(9094), new DateTime(2023, 1, 7, 18, 46, 38, 881, DateTimeKind.Local).AddTicks(9125) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "c9670008-da1a-40b7-a1cd-2bde465270c7", new DateTime(2023, 1, 7, 18, 46, 38, 881, DateTimeKind.Local).AddTicks(9199), new DateTime(2023, 1, 7, 18, 46, 38, 881, DateTimeKind.Local).AddTicks(9200) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "NormalizedName" },
                values: new object[] { "06bbd5c5-004b-448e-a973-2b2a920e4e64", new DateTime(2023, 1, 7, 18, 46, 38, 881, DateTimeKind.Local).AddTicks(9206), new DateTime(2023, 1, 7, 18, 46, 38, 881, DateTimeKind.Local).AddTicks(9208), "USER" });

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistAlbums_Albums_AlbumId",
                table: "ArtistAlbums",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistAlbums_Artists_ArtistId",
                table: "ArtistAlbums",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistSongs_Artists_ArtistId",
                table: "ArtistSongs",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistSongs_Songs_SongId",
                table: "ArtistSongs",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistSongs_Playlists_PlaylistId",
                table: "PlaylistSongs",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistSongs_Songs_SongId",
                table: "PlaylistSongs",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistUsers_AspNetUsers_UserId",
                table: "PlaylistUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistUsers_Playlists_PlaylistId",
                table: "PlaylistUsers",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Albums_AlbumId",
                table: "Songs",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtistAlbums_Albums_AlbumId",
                table: "ArtistAlbums");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistAlbums_Artists_ArtistId",
                table: "ArtistAlbums");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistSongs_Artists_ArtistId",
                table: "ArtistSongs");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistSongs_Songs_SongId",
                table: "ArtistSongs");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistSongs_Playlists_PlaylistId",
                table: "PlaylistSongs");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistSongs_Songs_SongId",
                table: "PlaylistSongs");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistUsers_AspNetUsers_UserId",
                table: "PlaylistUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistUsers_Playlists_PlaylistId",
                table: "PlaylistUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Albums_AlbumId",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "IsCreator",
                table: "PlaylistUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "75499802-10b0-44cb-a708-fa7ec9ae2cd6", new DateTime(2022, 12, 25, 16, 36, 45, 593, DateTimeKind.Local).AddTicks(1863), new DateTime(2022, 12, 25, 16, 36, 45, 593, DateTimeKind.Local).AddTicks(1902) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified" },
                values: new object[] { "4e9e8399-503e-48bc-a736-59fbfca34f23", new DateTime(2022, 12, 25, 16, 36, 45, 593, DateTimeKind.Local).AddTicks(1950), new DateTime(2022, 12, 25, 16, 36, 45, 593, DateTimeKind.Local).AddTicks(1951) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "NormalizedName" },
                values: new object[] { "4d656093-136a-4362-878a-dbfd781509d6", new DateTime(2022, 12, 25, 16, 36, 45, 593, DateTimeKind.Local).AddTicks(1958), new DateTime(2022, 12, 25, 16, 36, 45, 593, DateTimeKind.Local).AddTicks(1960), "User" });

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistAlbums_Albums_AlbumId",
                table: "ArtistAlbums",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistAlbums_Artists_ArtistId",
                table: "ArtistAlbums",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistSongs_Artists_ArtistId",
                table: "ArtistSongs",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistSongs_Songs_SongId",
                table: "ArtistSongs",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistSongs_Playlists_PlaylistId",
                table: "PlaylistSongs",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistSongs_Songs_SongId",
                table: "PlaylistSongs",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistUsers_AspNetUsers_UserId",
                table: "PlaylistUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistUsers_Playlists_PlaylistId",
                table: "PlaylistUsers",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Albums_AlbumId",
                table: "Songs",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id");
        }
    }
}
