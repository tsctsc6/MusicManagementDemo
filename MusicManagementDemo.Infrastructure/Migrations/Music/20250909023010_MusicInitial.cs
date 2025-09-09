using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace MusicManagementDemo.Infrastructure.Migrations.Music
{
    /// <inheritdoc />
    public partial class MusicInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "music");

            migrationBuilder.CreateTable(
                name: "MusicInfo",
                schema: "music",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Artist = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Album = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TitleTSV = table.Column<NpgsqlTsVector>(type: "TSVECTOR", nullable: false, computedColumnSql: "to_tsvector('english', \"Title\")", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MusicInfoMusicListMap",
                schema: "music",
                columns: table => new
                {
                    MusicInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    MusicListId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicInfoMusicListMap", x => new { x.MusicListId, x.MusicInfoId });
                });

            migrationBuilder.CreateTable(
                name: "MusicList",
                schema: "music",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicList", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MusicInfo_TitleTSV",
                schema: "music",
                table: "MusicInfo",
                column: "TitleTSV")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MusicInfo",
                schema: "music");

            migrationBuilder.DropTable(
                name: "MusicInfoMusicListMap",
                schema: "music");

            migrationBuilder.DropTable(
                name: "MusicList",
                schema: "music");
        }
    }
}
