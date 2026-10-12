using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlight.Migrations.Migrations.Airport
{
    /// <inheritdoc />
    public partial class RemovedUncessarryFiledFromSeasonal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcConfiguration",
                schema: "ops",
                table: "SeasonalFlightScheduleDepartureFlight");

            migrationBuilder.DropColumn(
                name: "DefaultGateCode",
                schema: "ops",
                table: "SeasonalFlightScheduleDepartureFlight");

            migrationBuilder.DropColumn(
                name: "DefaultParkPositionCode",
                schema: "ops",
                table: "SeasonalFlightScheduleDepartureFlight");

            migrationBuilder.DropColumn(
                name: "HandlingAgentCode",
                schema: "ops",
                table: "SeasonalFlightScheduleDepartureFlight");

            migrationBuilder.DropColumn(
                name: "TerminalCode",
                schema: "ops",
                table: "SeasonalFlightScheduleDepartureFlight");

            migrationBuilder.DropColumn(
                name: "AcConfiguration",
                schema: "ops",
                table: "SeasonalFlightScheduleArrivalFlight");

            migrationBuilder.DropColumn(
                name: "DefaultGateCode",
                schema: "ops",
                table: "SeasonalFlightScheduleArrivalFlight");

            migrationBuilder.DropColumn(
                name: "DefaultParkPositionCode",
                schema: "ops",
                table: "SeasonalFlightScheduleArrivalFlight");

            migrationBuilder.DropColumn(
                name: "HandlingAgentCode",
                schema: "ops",
                table: "SeasonalFlightScheduleArrivalFlight");

            migrationBuilder.DropColumn(
                name: "TerminalCode",
                schema: "ops",
                table: "SeasonalFlightScheduleArrivalFlight");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcConfiguration",
                schema: "ops",
                table: "SeasonalFlightScheduleDepartureFlight",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultGateCode",
                schema: "ops",
                table: "SeasonalFlightScheduleDepartureFlight",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultParkPositionCode",
                schema: "ops",
                table: "SeasonalFlightScheduleDepartureFlight",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HandlingAgentCode",
                schema: "ops",
                table: "SeasonalFlightScheduleDepartureFlight",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TerminalCode",
                schema: "ops",
                table: "SeasonalFlightScheduleDepartureFlight",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcConfiguration",
                schema: "ops",
                table: "SeasonalFlightScheduleArrivalFlight",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultGateCode",
                schema: "ops",
                table: "SeasonalFlightScheduleArrivalFlight",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultParkPositionCode",
                schema: "ops",
                table: "SeasonalFlightScheduleArrivalFlight",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HandlingAgentCode",
                schema: "ops",
                table: "SeasonalFlightScheduleArrivalFlight",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TerminalCode",
                schema: "ops",
                table: "SeasonalFlightScheduleArrivalFlight",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);
        }
    }
}
