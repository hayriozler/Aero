using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Aero.TextMessageParser.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class ParsedMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParsedMessages",
                schema: "messages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TextMessageId = table.Column<long>(type: "BIGINT", nullable: false),
                    MessageType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    AckRequest = table.Column<char>(type: "character(1)", nullable: true),
                    SourceIndicator = table.Column<char>(type: "character(1)", nullable: true),
                    SourceAirport = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    AirlineCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    FlightNumber = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    FlightDate = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Destination = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    AuthorityToLoad = table.Column<bool>(type: "boolean", nullable: true),
                    AuthorityToTransport = table.Column<bool>(type: "boolean", nullable: true),
                    BagTagStatus = table.Column<bool>(type: "boolean", nullable: true),
                    PassengerStatus = table.Column<char>(type: "character(1)", nullable: true),
                    PassengerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PassengerSequenceNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    PassengerSecurityNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    PassengerClass = table.Column<char>(type: "character(1)", nullable: true),
                    PassengerProfileStatus = table.Column<char>(type: "character(1)", nullable: true),
                    SeatNumber = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    WeightIndicator = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CheckedBagCount = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CheckedWeight = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    UncheckedWeight = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    WeightUnit = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    BagLength = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    BagWidth = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    BagHeight = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    BaggageTypeCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    RecordDateTime = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParsedMessageId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParsedMessages_TextMessages_TextMessageId",
                        column: x => x.TextMessageId,
                        principalSchema: "messages",
                        principalTable: "TextMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParsedBagTags",
                schema: "messages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParsedMessageId = table.Column<long>(type: "BIGINT", nullable: false),
                    BagTag = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParsedBagTagId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParsedBagTags_ParsedMessages_ParsedMessageId",
                        column: x => x.ParsedMessageId,
                        principalSchema: "messages",
                        principalTable: "ParsedMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParsedOnwardFlights",
                schema: "messages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParsedMessageId = table.Column<long>(type: "BIGINT", nullable: false),
                    AirlineCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    FlightNumber = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    FlightDate = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Destination = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Class = table.Column<char>(type: "character(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParsedOnwardFlightId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParsedOnwardFlights_ParsedMessages_ParsedMessageId",
                        column: x => x.ParsedMessageId,
                        principalSchema: "messages",
                        principalTable: "ParsedMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParsedBagTags_BagTag",
                schema: "messages",
                table: "ParsedBagTags",
                column: "BagTag");

            migrationBuilder.CreateIndex(
                name: "IX_ParsedBagTags_ParsedMessageId",
                schema: "messages",
                table: "ParsedBagTags",
                column: "ParsedMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ParsedMessages_AirlineCode_FlightNumber_FlightDate",
                schema: "messages",
                table: "ParsedMessages",
                columns: new[] { "AirlineCode", "FlightNumber", "FlightDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ParsedMessages_TextMessageId",
                schema: "messages",
                table: "ParsedMessages",
                column: "TextMessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParsedOnwardFlights_ParsedMessageId",
                schema: "messages",
                table: "ParsedOnwardFlights",
                column: "ParsedMessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParsedBagTags",
                schema: "messages");

            migrationBuilder.DropTable(
                name: "ParsedOnwardFlights",
                schema: "messages");

            migrationBuilder.DropTable(
                name: "ParsedMessages",
                schema: "messages");
        }
    }
}
