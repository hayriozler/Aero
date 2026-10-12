using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlight.Migrations.Migrations.Fids;

/// <inheritdoc />
public partial class AddDisplayClientTypeDropTerminal : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "TerminalCode",
            table: "DisplayClient");

        migrationBuilder.AddColumn<string>(
            name: "Type",
            table: "DisplayClient",
            type: "character varying(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Type",
            table: "DisplayClient");

        migrationBuilder.AddColumn<string>(
            name: "TerminalCode",
            table: "DisplayClient",
            type: "character varying(10)",
            maxLength: 10,
            nullable: true);
    }
}
