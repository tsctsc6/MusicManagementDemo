using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
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
            migrationBuilder.EnsureSchema(name: "music");

            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                schema: "music",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "MusicInfo",
                schema: "music",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(
                        type: "character varying(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    Artist = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Album = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    FilePath = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    TitleTSV = table.Column<NpgsqlTsVector>(
                        type: "TSVECTOR",
                        nullable: false,
                        computedColumnSql: "to_tsvector('english', \"Title\")",
                        stored: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicInfo", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "MusicInfoMusicListMap",
                schema: "music",
                columns: table => new
                {
                    MusicInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    MusicListId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "now()"
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_MusicInfoMusicListMap",
                        x => new { x.MusicListId, x.MusicInfoId }
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "MusicList",
                schema: "music",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    UserId = table.Column<string>(
                        type: "character varying(36)",
                        maxLength: 36,
                        nullable: false
                    ),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "now()"
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicList", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Storage",
                schema: "music",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    Name = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    Path = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storage", x => x.Id);
                }
            );

            migrationBuilder
                .CreateIndex(
                    name: "IX_MusicInfo_TitleTSV",
                    schema: "music",
                    table: "MusicInfo",
                    column: "TitleTSV"
                )
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_Storage_Name",
                schema: "music",
                table: "Storage",
                column: "Name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Storage_Path",
                schema: "music",
                table: "Storage",
                column: "Path",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ApplicationUser", schema: "music");

            migrationBuilder.DropTable(name: "MusicInfo", schema: "music");

            migrationBuilder.DropTable(name: "MusicInfoMusicListMap", schema: "music");

            migrationBuilder.DropTable(name: "MusicList", schema: "music");

            migrationBuilder.DropTable(name: "Storage", schema: "music");
        }
    }
}
