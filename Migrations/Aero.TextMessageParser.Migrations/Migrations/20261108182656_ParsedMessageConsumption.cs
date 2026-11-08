using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aero.TextMessageParser.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class ParsedMessageConsumption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConsumedAt",
                schema: "messages",
                table: "ParsedMessages",
                type: "TIMESTAMPTZ",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConsumed",
                schema: "messages",
                table: "ParsedMessages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ParsedMessages_Unconsumed",
                schema: "messages",
                table: "ParsedMessages",
                column: "Id",
                filter: "\"IsConsumed\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ParsedMessages_Unconsumed",
                schema: "messages",
                table: "ParsedMessages");

            migrationBuilder.DropColumn(
                name: "ConsumedAt",
                schema: "messages",
                table: "ParsedMessages");

            migrationBuilder.DropColumn(
                name: "IsConsumed",
                schema: "messages",
                table: "ParsedMessages");
        }
    }
}
