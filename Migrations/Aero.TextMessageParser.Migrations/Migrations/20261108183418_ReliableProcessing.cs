using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Aero.TextMessageParser.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class ReliableProcessing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttemptCount",
                schema: "messages",
                table: "TextMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 11);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "messages",
                table: "MessageErrors",
                type: "BIGINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "messages",
                table: "ElementErrors",
                type: "BIGINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_MessageErrors_MessageId",
                schema: "messages",
                table: "MessageErrors",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementErrors_MessageId",
                schema: "messages",
                table: "ElementErrors",
                column: "MessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ElementErrors_TextMessages_MessageId",
                schema: "messages",
                table: "ElementErrors",
                column: "MessageId",
                principalSchema: "messages",
                principalTable: "TextMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageErrors_TextMessages_MessageId",
                schema: "messages",
                table: "MessageErrors",
                column: "MessageId",
                principalSchema: "messages",
                principalTable: "TextMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElementErrors_TextMessages_MessageId",
                schema: "messages",
                table: "ElementErrors");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageErrors_TextMessages_MessageId",
                schema: "messages",
                table: "MessageErrors");

            migrationBuilder.DropIndex(
                name: "IX_MessageErrors_MessageId",
                schema: "messages",
                table: "MessageErrors");

            migrationBuilder.DropIndex(
                name: "IX_ElementErrors_MessageId",
                schema: "messages",
                table: "ElementErrors");

            migrationBuilder.DropColumn(
                name: "AttemptCount",
                schema: "messages",
                table: "TextMessages");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "messages",
                table: "MessageErrors",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "messages",
                table: "ElementErrors",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
