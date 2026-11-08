using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aero.TextMessageParser.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class RenameTimestampColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecordDateTime",
                schema: "messages",
                table: "TextMessages",
                newName: "ReceivedAtUtc");

            migrationBuilder.RenameColumn(
                name: "ProcessDateTime",
                schema: "messages",
                table: "TextMessages",
                newName: "ProcessedAtUtc");

            migrationBuilder.RenameColumn(
                name: "RecordDateTime",
                schema: "messages",
                table: "ParsedMessages",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "RecordDateTime",
                schema: "messages",
                table: "MessageErrors",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "RecordDateTime",
                schema: "messages",
                table: "ElementErrors",
                newName: "CreatedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceivedAtUtc",
                schema: "messages",
                table: "TextMessages",
                newName: "RecordDateTime");

            migrationBuilder.RenameColumn(
                name: "ProcessedAtUtc",
                schema: "messages",
                table: "TextMessages",
                newName: "ProcessDateTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                schema: "messages",
                table: "ParsedMessages",
                newName: "RecordDateTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                schema: "messages",
                table: "MessageErrors",
                newName: "RecordDateTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                schema: "messages",
                table: "ElementErrors",
                newName: "RecordDateTime");
        }
    }
}
