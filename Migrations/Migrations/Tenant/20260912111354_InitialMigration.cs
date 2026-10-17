using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlight.Migrations.Migrations.Tenant;

/// <inheritdoc />
public partial class InitialMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:PostgresExtension:citext", ",,");

        migrationBuilder.CreateTable(
            name: "Tenant",
            columns: table => new
            {
                Id = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                ConnectionStringProtected = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                HomeAirportCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tenant", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ClientApp",
            columns: table => new
            {
                Name = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                TenantId = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                Secret = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                BeginDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                EndDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ClientApp", x => new { x.Name, x.TenantId });
                table.ForeignKey(
                    name: "FK_ClientApp_Tenant_TenantId",
                    column: x => x.TenantId,
                    principalTable: "Tenant",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "TenantInterface",
            columns: table => new
            {
                TenantId = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                InterfaceCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TenantInterface", x => new { x.TenantId, x.InterfaceCode });
                table.ForeignKey(
                    name: "FK_TenantInterface_Tenant_TenantId",
                    column: x => x.TenantId,
                    principalTable: "Tenant",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ClientApp_TenantId",
            table: "ClientApp",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_Tenant_IsActive",
            table: "Tenant",
            column: "IsActive");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ClientApp");

        migrationBuilder.DropTable(
            name: "TenantInterface");

        migrationBuilder.DropTable(
            name: "Tenant");
    }
}
