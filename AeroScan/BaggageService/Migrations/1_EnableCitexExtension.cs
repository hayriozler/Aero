using Microsoft.EntityFrameworkCore.Migrations;

namespace BaggageService.Migrations;

public class EnableCitexExtension : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS citext;");
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("DROP EXTENSION IF NOT EXISTS citext;");
}
