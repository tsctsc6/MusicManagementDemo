using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MusicManagementDemo.Infrastructure.Migrations.Management
{
    /// <inheritdoc />
    public partial class ManagementInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "management");

            migrationBuilder.CreateTable(
                name: "Job",
                schema: "management",
                columns: table => new
                {
                    Id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    Type = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    JobArgs = table.Column<string>(type: "jsonb", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Description = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: false
                    ),
                    ErrorMesage = table.Column<string>(type: "text", nullable: false),
                    Success = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        defaultValue: false
                    ),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "now()"
                    ),
                    CompletedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Storage",
                schema: "management",
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

            migrationBuilder.CreateIndex(
                name: "IX_Storage_Name",
                schema: "management",
                table: "Storage",
                column: "Name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Storage_Path",
                schema: "management",
                table: "Storage",
                column: "Path",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Job", schema: "management");

            migrationBuilder.DropTable(name: "Storage", schema: "management");
        }
    }
}
