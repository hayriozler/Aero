using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aero.TextMessageParser.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class PartialIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TextMessages_Status_Completed",
                schema: "messages",
                table: "TextMessages");

            migrationBuilder.DropColumn(
                name: "Completed",
                schema: "messages",
                table: "TextMessages");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RecordDateTime",
                schema: "messages",
                table: "TextMessages",
                type: "TIMESTAMPTZ",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ")
                .Annotation("Relational:ColumnOrder", 10)
                .OldAnnotation("Relational:ColumnOrder", 11);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProcessDateTime",
                schema: "messages",
                table: "TextMessages",
                type: "TIMESTAMPTZ",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 9)
                .OldAnnotation("Relational:ColumnOrder", 10);

            migrationBuilder.CreateIndex(
                name: "IX_TextMessages_Pending",
                schema: "messages",
                table: "TextMessages",
                column: "RecordDateTime",
                filter: "\"Status\" = 'NOT_PROCESSED'");

            migrationBuilder.CreateIndex(
                name: "IX_TextMessages_Processing",
                schema: "messages",
                table: "TextMessages",
                column: "ProcessingStartedAt",
                filter: "\"Status\" = 'PROCESSING'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TextMessages_Pending",
                schema: "messages",
                table: "TextMessages");

            migrationBuilder.DropIndex(
                name: "IX_TextMessages_Processing",
                schema: "messages",
                table: "TextMessages");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RecordDateTime",
                schema: "messages",
                table: "TextMessages",
                type: "TIMESTAMPTZ",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ")
                .Annotation("Relational:ColumnOrder", 11)
                .OldAnnotation("Relational:ColumnOrder", 10);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProcessDateTime",
                schema: "messages",
                table: "TextMessages",
                type: "TIMESTAMPTZ",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 10)
                .OldAnnotation("Relational:ColumnOrder", 9);

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                schema: "messages",
                table: "TextMessages",
                type: "boolean",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:ColumnOrder", 9);

            migrationBuilder.CreateIndex(
                name: "IX_TextMessages_Status_Completed",
                schema: "messages",
                table: "TextMessages",
                columns: new[] { "Status", "Completed" });
        }
    }
}
