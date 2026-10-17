using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlight.Migrations.Migrations.Airport;

/// <inheritdoc />
public partial class RemoveSeasonalPreviousNextAirportCode : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "NextAirportCode",
            schema: "ops",
            table: "SeasonalFlightScheduleDepartureFlight");

        migrationBuilder.DropColumn(
            name: "PreviousAirportCode",
            schema: "ops",
            table: "SeasonalFlightScheduleArrivalFlight");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "NextAirportCode",
            schema: "ops",
            table: "SeasonalFlightScheduleDepartureFlight",
            type: "character varying(4)",
            maxLength: 4,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "PreviousAirportCode",
            schema: "ops",
            table: "SeasonalFlightScheduleArrivalFlight",
            type: "character varying(4)",
            maxLength: 4,
            nullable: true);
    }
}
