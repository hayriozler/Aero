using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlight.Migrations.Migrations.Fids;

/// <inheritdoc />
public partial class AddDisplayClientResourceCode : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ResourceCode",
            table: "DisplayClient",
            type: "character varying(20)",
            maxLength: 20,
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ResourceCode",
            table: "DisplayClient");
    }
}
