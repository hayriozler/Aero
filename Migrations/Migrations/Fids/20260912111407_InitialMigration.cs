using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AeroFlight.Migrations.Migrations.Fids;

/// <inheritdoc />
public partial class InitialMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "DisplayClient",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                TerminalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                BankName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                BankPosition = table.Column<int>(type: "integer", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DisplayClient", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "FidsSetting",
            columns: table => new
            {
                Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Value = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FidsSetting", x => x.Key);
            });

        migrationBuilder.CreateTable(
            name: "FidsUser",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                PasswordHash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FidsUser", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "FieldMapping",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                TargetField = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                FlightType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                SourcePath = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FieldMapping", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "StatusMapping",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Dimension = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                FlightType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                ExternalValue = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                InternalValue = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StatusMapping", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_DisplayClient_Name",
            table: "DisplayClient",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_FidsUser_Username",
            table: "FidsUser",
            column: "Username",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_FieldMapping_TargetField_FlightType",
            table: "FieldMapping",
            columns: new[] { "TargetField", "FlightType" });

        migrationBuilder.CreateIndex(
            name: "IX_StatusMapping_Dimension_FlightType_ExternalValue",
            table: "StatusMapping",
            columns: new[] { "Dimension", "FlightType", "ExternalValue" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DisplayClient");

        migrationBuilder.DropTable(
            name: "FidsSetting");

        migrationBuilder.DropTable(
            name: "FidsUser");

        migrationBuilder.DropTable(
            name: "FieldMapping");

        migrationBuilder.DropTable(
            name: "StatusMapping");
    }
}
