using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AeroFlight.Migrations.Migrations.Tenant;

/// <inheritdoc />
public partial class AddTenantInterfaceSetting : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TenantInterfaceSetting",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                TenantId = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Value = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TenantInterfaceSetting", x => x.Id);
                table.ForeignKey(
                    name: "FK_TenantInterfaceSetting_Tenant_TenantId",
                    column: x => x.TenantId,
                    principalTable: "Tenant",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_TenantInterfaceSetting_TenantId_Key",
            table: "TenantInterfaceSetting",
            columns: new[] { "TenantId", "Key" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "TenantInterfaceSetting");
    }
}
