using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlight.Migrations.Migrations.Fids;

/// <inheritdoc />
public partial class AddFieldMappingRule : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "RuleOperator",
            table: "FieldMapping",
            type: "character varying(20)",
            maxLength: 20,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "RuleValue",
            table: "FieldMapping",
            type: "character varying(200)",
            maxLength: 200,
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RuleOperator",
            table: "FieldMapping");

        migrationBuilder.DropColumn(
            name: "RuleValue",
            table: "FieldMapping");
    }
}
