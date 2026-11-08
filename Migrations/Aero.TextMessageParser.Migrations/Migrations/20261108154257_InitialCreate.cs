using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Aero.TextMessageParser.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "messages");

            migrationBuilder.CreateTable(
                name: "ElementErrors",
                schema: "messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageId = table.Column<long>(type: "BIGINT", nullable: false),
                    ErrorCode = table.Column<string>(type: "VARCHAR", maxLength: 50, nullable: false),
                    ErrorDescription = table.Column<string>(type: "VARCHAR", maxLength: 250, nullable: true),
                    RecordDateTime = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementErrorId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageErrors",
                schema: "messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageId = table.Column<long>(type: "BIGINT", nullable: false),
                    ErrorCode = table.Column<string>(type: "VARCHAR", maxLength: 50, nullable: false),
                    ErrorDescription = table.Column<string>(type: "VARCHAR", maxLength: 250, nullable: true),
                    RecordDateTime = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageErrorId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TextMessages",
                schema: "messages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Header_Identifier = table.Column<string>(type: "VARCHAR", maxLength: 3, nullable: false),
                    Header_SecondaryIdentifier = table.Column<string>(type: "VARCHAR", maxLength: 3, nullable: true),
                    Header_ChangeOfStatus = table.Column<string>(type: "VARCHAR", maxLength: 3, nullable: true),
                    Footer_EndIdentifier = table.Column<string>(type: "VARCHAR", maxLength: 6, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    ProcessingStartedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: true),
                    Completed = table.Column<bool>(type: "boolean", nullable: false),
                    ProcessDateTime = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: true),
                    RecordDateTime = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextMessageId", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TextMessages_Header_Identifier_Header_SecondaryIdentifier_H~",
                schema: "messages",
                table: "TextMessages",
                columns: new[] { "Header_Identifier", "Header_SecondaryIdentifier", "Header_ChangeOfStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_TextMessages_Status_Completed",
                schema: "messages",
                table: "TextMessages",
                columns: new[] { "Status", "Completed" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElementErrors",
                schema: "messages");

            migrationBuilder.DropTable(
                name: "MessageErrors",
                schema: "messages");

            migrationBuilder.DropTable(
                name: "TextMessages",
                schema: "messages");
        }
    }
}
