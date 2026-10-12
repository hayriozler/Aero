using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlight.Migrations.Migrations.Airport;

/// <inheritdoc />
public partial class AddTerminalCodeToCounterGateBelts : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Gate_Airline_AirlineCode",
            schema: "metadata",
            table: "Gate");

        migrationBuilder.DropIndex(
            name: "IX_Gate_AirlineCode",
            schema: "metadata",
            table: "Gate");

        migrationBuilder.DropColumn(
            name: "AirlineCode",
            schema: "metadata",
            table: "Gate");

        migrationBuilder.DropColumn(
            name: "CounterGroup",
            schema: "metadata",
            table: "Counter");

        migrationBuilder.DropColumn(
            name: "TypeCode",
            schema: "metadata",
            table: "Counter");

        migrationBuilder.AddColumn<string>(
            name: "TerminalCode",
            schema: "metadata",
            table: "Gate",
            type: "citext",
            maxLength: 1,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "TerminalCode",
            schema: "metadata",
            table: "DepartureBelt",
            type: "citext",
            maxLength: 10,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "TerminalCode",
            schema: "metadata",
            table: "Counter",
            type: "citext",
            maxLength: 10,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "TerminalCode",
            schema: "metadata",
            table: "ArrivalBelt",
            type: "citext",
            maxLength: 10,
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Gate_TerminalCode",
            schema: "metadata",
            table: "Gate",
            column: "TerminalCode");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureBelt_TerminalCode",
            schema: "metadata",
            table: "DepartureBelt",
            column: "TerminalCode");

        migrationBuilder.CreateIndex(
            name: "IX_Counter_TerminalCode",
            schema: "metadata",
            table: "Counter",
            column: "TerminalCode");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalBelt_TerminalCode",
            schema: "metadata",
            table: "ArrivalBelt",
            column: "TerminalCode");

        migrationBuilder.AddForeignKey(
            name: "FK_ArrivalBelt_Terminal_TerminalCode",
            schema: "metadata",
            table: "ArrivalBelt",
            column: "TerminalCode",
            principalSchema: "metadata",
            principalTable: "Terminal",
            principalColumn: "Code");

        migrationBuilder.AddForeignKey(
            name: "FK_Counter_Terminal_TerminalCode",
            schema: "metadata",
            table: "Counter",
            column: "TerminalCode",
            principalSchema: "metadata",
            principalTable: "Terminal",
            principalColumn: "Code");

        migrationBuilder.AddForeignKey(
            name: "FK_DepartureBelt_Terminal_TerminalCode",
            schema: "metadata",
            table: "DepartureBelt",
            column: "TerminalCode",
            principalSchema: "metadata",
            principalTable: "Terminal",
            principalColumn: "Code");

        migrationBuilder.AddForeignKey(
            name: "FK_Gate_Terminal_TerminalCode",
            schema: "metadata",
            table: "Gate",
            column: "TerminalCode",
            principalSchema: "metadata",
            principalTable: "Terminal",
            principalColumn: "Code");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ArrivalBelt_Terminal_TerminalCode",
            schema: "metadata",
            table: "ArrivalBelt");

        migrationBuilder.DropForeignKey(
            name: "FK_Counter_Terminal_TerminalCode",
            schema: "metadata",
            table: "Counter");

        migrationBuilder.DropForeignKey(
            name: "FK_DepartureBelt_Terminal_TerminalCode",
            schema: "metadata",
            table: "DepartureBelt");

        migrationBuilder.DropForeignKey(
            name: "FK_Gate_Terminal_TerminalCode",
            schema: "metadata",
            table: "Gate");

        migrationBuilder.DropIndex(
            name: "IX_Gate_TerminalCode",
            schema: "metadata",
            table: "Gate");

        migrationBuilder.DropIndex(
            name: "IX_DepartureBelt_TerminalCode",
            schema: "metadata",
            table: "DepartureBelt");

        migrationBuilder.DropIndex(
            name: "IX_Counter_TerminalCode",
            schema: "metadata",
            table: "Counter");

        migrationBuilder.DropIndex(
            name: "IX_ArrivalBelt_TerminalCode",
            schema: "metadata",
            table: "ArrivalBelt");

        migrationBuilder.DropColumn(
            name: "TerminalCode",
            schema: "metadata",
            table: "Gate");

        migrationBuilder.DropColumn(
            name: "TerminalCode",
            schema: "metadata",
            table: "DepartureBelt");

        migrationBuilder.DropColumn(
            name: "TerminalCode",
            schema: "metadata",
            table: "Counter");

        migrationBuilder.DropColumn(
            name: "TerminalCode",
            schema: "metadata",
            table: "ArrivalBelt");

        migrationBuilder.AddColumn<string>(
            name: "AirlineCode",
            schema: "metadata",
            table: "Gate",
            type: "citext",
            maxLength: 3,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "CounterGroup",
            schema: "metadata",
            table: "Counter",
            type: "character varying(20)",
            maxLength: 20,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "TypeCode",
            schema: "metadata",
            table: "Counter",
            type: "character varying(10)",
            maxLength: 10,
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Gate_AirlineCode",
            schema: "metadata",
            table: "Gate",
            column: "AirlineCode");

        migrationBuilder.AddForeignKey(
            name: "FK_Gate_Airline_AirlineCode",
            schema: "metadata",
            table: "Gate",
            column: "AirlineCode",
            principalSchema: "metadata",
            principalTable: "Airline",
            principalColumn: "Code");
    }
}
