using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AeroFlight.Migrations.Migrations.Airport;

/// <inheritdoc />
public partial class InitialMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "ops");

        migrationBuilder.EnsureSchema(
            name: "audits");

        migrationBuilder.EnsureSchema(
            name: "metadata");

        migrationBuilder.EnsureSchema(
            name: "identity");

        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:PostgresExtension:citext", ",,");

        migrationBuilder.CreateSequence(
            name: "DepartureFlightSequence");

        migrationBuilder.CreateSequence(
            name: "DepartureRoutingSequence");

        migrationBuilder.CreateSequence(
            name: "FlightResourceSequence");

        migrationBuilder.CreateSequence(
            name: "SeasonalFlightScheduleDepartureFlightRoutingSequence");

        migrationBuilder.CreateSequence(
            name: "SeasonalFlightScheduleDepartureFlightSequence");

        migrationBuilder.CreateTable(
            name: "AeroConfiguration",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ConfigurationName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                ConfigType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                IsVisible = table.Column<bool>(type: "boolean", nullable: false),
                IsUpdateable = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AeroConfiguration", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AeroConfigurationAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AeroConfigurationAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AircraftAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AircraftAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AircraftNoiseCategory",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AircraftNoiseCategory", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "AircraftSizeCategory",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 1, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AircraftSizeCategory", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "AircraftSizeCategoryAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AircraftSizeCategoryAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AircraftTypeAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AircraftTypeAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AirlineAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AirlineAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AirportAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AirportAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ArrivalBelt",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                Capacity = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArrivalBelt", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "ArrivalBeltAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArrivalBeltAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ArrivalBeltStatus",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                IsOpenValue = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                IsCloseValue = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArrivalBeltStatus", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "ArrivalFlightAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArrivalFlightAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ArrivalMovementMessage",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                MessageType = table.Column<string>(type: "text", nullable: false),
                Content = table.Column<string>(type: "text", nullable: false),
                ReceivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArrivalMovementMessage", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CheckinClass",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                Description = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CheckinClass", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "CheckinClassAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CheckinClassAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CompanyAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CompanyAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CompanyType",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 20, nullable: false),
                Description = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CompanyType", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "CompanyTypeAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CompanyTypeAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Counter",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 5, nullable: false),
                TypeCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                CounterGroup = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                Capacity = table.Column<int>(type: "integer", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Counter", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "CounterAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CounterAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CounterStatus",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 1, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                IsOpenValue = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                IsCloseValue = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CounterStatus", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "CountryAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CountryAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CountryType",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 1, nullable: false),
                Description = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Severity = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CountryType", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "DeBoardingStatus",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 1, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DeBoardingStatus", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "DelayReason",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 4, nullable: false),
                AlphabeticCode = table.Column<string>(type: "citext", maxLength: 5, nullable: true),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DelayReason", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "DelayReasonAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DelayReasonAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "DepartureBelt",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                Capacity = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DepartureBelt", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "DepartureBeltAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DepartureBeltAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "DepartureBeltStatus",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                IsOpenValue = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                IsCloseValue = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DepartureBeltStatus", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "DepartureFlightAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DepartureFlightAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "DepartureMovementMessage",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                MessageType = table.Column<string>(type: "text", nullable: false),
                Content = table.Column<string>(type: "text", nullable: false),
                ReceivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DepartureMovementMessage", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "FlightStatus",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 5, nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                IsUpdatable = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FlightStatus", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "FlightType",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 1, nullable: false),
                Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                DisplayCode = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FlightType", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "GateAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GateAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "GateStatus",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 1, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                IsOpenValue = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                IsCloseValue = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GateStatus", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "GateType",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 1, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GateType", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "HandlingService",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 5, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                Unit = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HandlingService", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "HandlingServiceAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HandlingServiceAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "InboxMessage",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Source = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                MessageType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Payload = table.Column<string>(type: "text", nullable: false),
                ReceivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ProcessedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Attempts = table.Column<int>(type: "integer", nullable: false),
                LastError = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InboxMessage", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "JobSchedule",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                JobName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                CronExpression = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                IntervalSeconds = table.Column<int>(type: "integer", nullable: true),
                IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_JobSchedule", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "OpsArrivalBeltAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpsArrivalBeltAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "OpsArrivalGateAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpsArrivalGateAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "OpsCounterAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpsCounterAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "OpsDepartureBeltAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpsDepartureBeltAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "OpsDepartureGateAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpsDepartureGateAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "OpsFlightParkPositionAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpsFlightParkPositionAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "OutboxMessage",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Exchange = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                RoutingKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                MessageType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Payload = table.Column<string>(type: "text", nullable: false),
                OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                PublishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Attempts = table.Column<int>(type: "integer", nullable: false),
                LastError = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OutboxMessage", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ParkPositionAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ParkPositionAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ParkPositionType",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 1, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ParkPositionType", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "Permission",
            schema: "identity",
            columns: table => new
            {
                Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Permission", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "Region",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Region", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "Remark",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 20, nullable: false),
                Description = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Remark", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "RemarkAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RemarkAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Role",
            schema: "identity",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Role", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "RoleAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RoleAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Rule",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                StopOnMatch = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Rule", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "RuleActionType",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RuleActionType", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "RuleField",
            schema: "metadata",
            columns: table => new
            {
                Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                DataType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                UsableInCondition = table.Column<bool>(type: "boolean", nullable: false),
                UsableInAction = table.Column<bool>(type: "boolean", nullable: false),
                SupportsMatchValue = table.Column<bool>(type: "boolean", nullable: false),
                MatchValueLabel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RuleField", x => x.Key);
            });

        migrationBuilder.CreateTable(
            name: "RuleFieldActionType",
            schema: "metadata",
            columns: table => new
            {
                RuleFieldKey = table.Column<string>(type: "text", nullable: false),
                ActionTypeCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RuleFieldActionType", x => new { x.RuleFieldKey, x.ActionTypeCode });
            });

        migrationBuilder.CreateTable(
            name: "RuleFieldTrigger",
            schema: "metadata",
            columns: table => new
            {
                RuleFieldKey = table.Column<string>(type: "text", nullable: false),
                Trigger = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RuleFieldTrigger", x => new { x.RuleFieldKey, x.Trigger });
            });

        migrationBuilder.CreateTable(
            name: "RuleTriggerOption",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RuleTriggerOption", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "Runway",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Length = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                Width = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Runway", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "RunwayAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RunwayAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ScheduleSeason",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                BeginDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ScheduleSeason", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "SystemProperty",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SystemProperty", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Terminal",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 5, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Terminal", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "TerminalAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TerminalAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Timezone",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Timezone", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "User",
            schema: "identity",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Username = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                RealName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                PasswordExpirationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                MultipleLoginsAllowed = table.Column<int>(type: "integer", nullable: true),
                ExpirationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_User", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserAudit",
            schema: "audits",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserAudit", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AircraftType",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 6, nullable: false),
                IataCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                IataSubCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                IcaoCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                SizeCategory = table.Column<string>(type: "citext", maxLength: 1, nullable: true),
                NoiseCategory = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                Manufacturer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Mtow = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                LandingWeight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                SeatCapacity = table.Column<int>(type: "integer", nullable: true),
                IsBridgeRequired = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AircraftType", x => x.Code);
                table.ForeignKey(
                    name: "FK_AircraftType_AircraftNoiseCategory_NoiseCategory",
                    column: x => x.NoiseCategory,
                    principalSchema: "metadata",
                    principalTable: "AircraftNoiseCategory",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_AircraftType_AircraftSizeCategory_SizeCategory",
                    column: x => x.SizeCategory,
                    principalSchema: "metadata",
                    principalTable: "AircraftSizeCategory",
                    principalColumn: "Code");
            });

        migrationBuilder.CreateTable(
            name: "Company",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                CompanyTypeCode = table.Column<string>(type: "citext", maxLength: 20, nullable: true),
                TaxNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                BillingAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                BankAccount = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                InvoiceCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Company", x => x.Code);
                table.ForeignKey(
                    name: "FK_Company_CompanyType_CompanyTypeCode",
                    column: x => x.CompanyTypeCode,
                    principalSchema: "metadata",
                    principalTable: "CompanyType",
                    principalColumn: "Code");
            });

        migrationBuilder.CreateTable(
            name: "ParkPosition",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                TypeCode = table.Column<string>(type: "citext", maxLength: 1, nullable: true),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                Capacity = table.Column<int>(type: "integer", nullable: true),
                Latitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: true),
                Longitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: true),
                Angle = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ParkPosition", x => x.Code);
                table.ForeignKey(
                    name: "FK_ParkPosition_ParkPositionType_TypeCode",
                    column: x => x.TypeCode,
                    principalSchema: "metadata",
                    principalTable: "ParkPositionType",
                    principalColumn: "Code");
            });

        migrationBuilder.CreateTable(
            name: "Country",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 5, nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                CountryTypeCode = table.Column<string>(type: "citext", maxLength: 1, nullable: false),
                RegionCode = table.Column<string>(type: "citext", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Country", x => x.Code);
                table.ForeignKey(
                    name: "FK_Country_CountryType_CountryTypeCode",
                    column: x => x.CountryTypeCode,
                    principalSchema: "metadata",
                    principalTable: "CountryType",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Country_Region_RegionCode",
                    column: x => x.RegionCode,
                    principalSchema: "metadata",
                    principalTable: "Region",
                    principalColumn: "Code");
            });

        migrationBuilder.CreateTable(
            name: "RolePermission",
            schema: "identity",
            columns: table => new
            {
                RoleId = table.Column<int>(type: "integer", nullable: false),
                PermissionCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RolePermission", x => new { x.RoleId, x.PermissionCode });
                table.ForeignKey(
                    name: "FK_RolePermission_Permission_PermissionCode",
                    column: x => x.PermissionCode,
                    principalSchema: "identity",
                    principalTable: "Permission",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_RolePermission_Role_RoleId",
                    column: x => x.RoleId,
                    principalSchema: "identity",
                    principalTable: "Role",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "RuleAction",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                RuleId = table.Column<int>(type: "integer", nullable: false),
                ActionType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                TargetField = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                MatchValue = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Order = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RuleAction", x => x.Id);
                table.ForeignKey(
                    name: "FK_RuleAction_Rule_RuleId",
                    column: x => x.RuleId,
                    principalSchema: "ops",
                    principalTable: "Rule",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "RuleConditionGroup",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                RuleId = table.Column<int>(type: "integer", nullable: false),
                Order = table.Column<int>(type: "integer", nullable: false),
                Operator = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RuleConditionGroup", x => x.Id);
                table.ForeignKey(
                    name: "FK_RuleConditionGroup_Rule_RuleId",
                    column: x => x.RuleId,
                    principalSchema: "ops",
                    principalTable: "Rule",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "RuleTrigger",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                RuleId = table.Column<int>(type: "integer", nullable: false),
                Trigger = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RuleTrigger", x => x.Id);
                table.ForeignKey(
                    name: "FK_RuleTrigger_Rule_RuleId",
                    column: x => x.RuleId,
                    principalSchema: "ops",
                    principalTable: "Rule",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SeasonalFlightScheduleArrivalFlight",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"SeasonalFlightScheduleDepartureFlightSequence\"')"),
                ScheduleSeasonCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                AirlineCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                FlightNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                CallSign = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                ScheduleFlightStatusCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                CountryType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                AircraftTypeCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                AcConfiguration = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                OpsDays = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                Frequency = table.Column<int>(type: "integer", nullable: false),
                IsOverMidnight = table.Column<bool>(type: "boolean", nullable: false),
                TerminalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                DefaultGateCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                DefaultParkPositionCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                HandlingAgentCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                DirectionCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                IsTransit = table.Column<bool>(type: "boolean", nullable: false),
                Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                Sta = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                PeriodBegin = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                PeriodEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                OriginAirportCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                PreviousAirportCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                LinkedDepartureFlightId = table.Column<int>(type: "integer", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SeasonalFlightScheduleArrivalFlight", x => x.Id);
                table.ForeignKey(
                    name: "FK_SeasonalFlightScheduleArrivalFlight_ScheduleSeason_Schedule~",
                    column: x => x.ScheduleSeasonCode,
                    principalSchema: "ops",
                    principalTable: "ScheduleSeason",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SeasonalFlightScheduleDepartureFlight",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"SeasonalFlightScheduleDepartureFlightSequence\"')"),
                ScheduleSeasonCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                AirlineCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                FlightNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                CallSign = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                ScheduleFlightStatusCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                CountryType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                AircraftTypeCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                AcConfiguration = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                OpsDays = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                Frequency = table.Column<int>(type: "integer", nullable: false),
                IsOverMidnight = table.Column<bool>(type: "boolean", nullable: false),
                TerminalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                DefaultGateCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                DefaultParkPositionCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                HandlingAgentCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                DirectionCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                IsTransit = table.Column<bool>(type: "boolean", nullable: false),
                Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                Std = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Ptd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                PeriodBegin = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                PeriodEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DestinationAirportCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                NextAirportCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                LinkedArrivalFlightId = table.Column<int>(type: "integer", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SeasonalFlightScheduleDepartureFlight", x => x.Id);
                table.ForeignKey(
                    name: "FK_SeasonalFlightScheduleDepartureFlight_ScheduleSeason_Schedu~",
                    column: x => x.ScheduleSeasonCode,
                    principalSchema: "ops",
                    principalTable: "ScheduleSeason",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UserRole",
            schema: "identity",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<int>(type: "integer", nullable: false),
                RoleId = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserRole", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserRole_Role_RoleId",
                    column: x => x.RoleId,
                    principalSchema: "identity",
                    principalTable: "Role",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_UserRole_User_UserId",
                    column: x => x.UserId,
                    principalSchema: "identity",
                    principalTable: "User",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CompanyContact",
            schema: "metadata",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                CompanyCode = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                ContactType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                Value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CompanyContact", x => x.Id);
                table.ForeignKey(
                    name: "FK_CompanyContact_Company_CompanyCode",
                    column: x => x.CompanyCode,
                    principalSchema: "metadata",
                    principalTable: "Company",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Airline",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 3, nullable: false),
                IataCode = table.Column<string>(type: "citext", maxLength: 3, nullable: false),
                IcaoCode = table.Column<string>(type: "citext", maxLength: 4, nullable: true),
                FlightNumberCode = table.Column<string>(type: "citext", maxLength: 3, nullable: true),
                Callsign = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                ParentAirlineCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                CountryCode = table.Column<string>(type: "citext", maxLength: 3, nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Airline", x => x.Code);
                table.ForeignKey(
                    name: "FK_Airline_Airline_ParentAirlineCode",
                    column: x => x.ParentAirlineCode,
                    principalSchema: "metadata",
                    principalTable: "Airline",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_Airline_Country_CountryCode",
                    column: x => x.CountryCode,
                    principalSchema: "metadata",
                    principalTable: "Country",
                    principalColumn: "Code");
            });

        migrationBuilder.CreateTable(
            name: "Airport",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 3, nullable: false),
                IataCode = table.Column<string>(type: "citext", maxLength: 3, nullable: true),
                IcaoCode = table.Column<string>(type: "citext", maxLength: 4, nullable: true),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                CountryCode = table.Column<string>(type: "citext", maxLength: 3, nullable: true),
                TimezoneCode = table.Column<string>(type: "citext", maxLength: 50, nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Airport", x => x.Code);
                table.ForeignKey(
                    name: "FK_Airport_Country_CountryCode",
                    column: x => x.CountryCode,
                    principalSchema: "metadata",
                    principalTable: "Country",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_Airport_Timezone_TimezoneCode",
                    column: x => x.TimezoneCode,
                    principalSchema: "metadata",
                    principalTable: "Timezone",
                    principalColumn: "Code");
            });

        migrationBuilder.CreateTable(
            name: "RuleCondition",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                GroupId = table.Column<int>(type: "integer", nullable: false),
                Field = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Operator = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RuleCondition", x => x.Id);
                table.ForeignKey(
                    name: "FK_RuleCondition_RuleConditionGroup_GroupId",
                    column: x => x.GroupId,
                    principalSchema: "ops",
                    principalTable: "RuleConditionGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SeasonalFlightScheduleArrivalFlightRouting",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"SeasonalFlightScheduleDepartureFlightRoutingSequence\"')"),
                SeasonalFlightScheduleFlightId = table.Column<int>(type: "integer", nullable: false),
                AirportCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                LegNumber = table.Column<int>(type: "integer", nullable: false),
                DirectionCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                Sta = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Std = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SeasonalFlightScheduleArrivalFlightRouting", x => x.Id);
                table.ForeignKey(
                    name: "FK_SeasonalFlightScheduleArrivalFlightRouting_SeasonalFlightSc~",
                    column: x => x.SeasonalFlightScheduleFlightId,
                    principalSchema: "ops",
                    principalTable: "SeasonalFlightScheduleArrivalFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SeasonalFlightScheduleDepartureFlightRouting",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"SeasonalFlightScheduleDepartureFlightRoutingSequence\"')"),
                SeasonalFlightScheduleFlightId = table.Column<int>(type: "integer", nullable: false),
                AirportCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                LegNumber = table.Column<int>(type: "integer", nullable: false),
                DirectionCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                Sta = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Std = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SeasonalFlightScheduleDepartureFlightRouting", x => x.Id);
                table.ForeignKey(
                    name: "FK_SeasonalFlightScheduleDepartureFlightRouting_SeasonalFlight~",
                    column: x => x.SeasonalFlightScheduleFlightId,
                    principalSchema: "ops",
                    principalTable: "SeasonalFlightScheduleDepartureFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Aircraft",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                AircraftTypeCode = table.Column<string>(type: "citext", maxLength: 5, nullable: false),
                AirlineCode = table.Column<string>(type: "citext", maxLength: 3, nullable: true),
                Owner = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                SizeCategory = table.Column<string>(type: "citext", maxLength: 1, nullable: true),
                NoiseCategory = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                Weight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                SeatCapacity = table.Column<int>(type: "integer", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Aircraft", x => x.Code);
                table.ForeignKey(
                    name: "FK_Aircraft_AircraftNoiseCategory_NoiseCategory",
                    column: x => x.NoiseCategory,
                    principalSchema: "metadata",
                    principalTable: "AircraftNoiseCategory",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_Aircraft_AircraftSizeCategory_SizeCategory",
                    column: x => x.SizeCategory,
                    principalSchema: "metadata",
                    principalTable: "AircraftSizeCategory",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_Aircraft_AircraftType_AircraftTypeCode",
                    column: x => x.AircraftTypeCode,
                    principalSchema: "metadata",
                    principalTable: "AircraftType",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Aircraft_Airline_AirlineCode",
                    column: x => x.AirlineCode,
                    principalSchema: "metadata",
                    principalTable: "Airline",
                    principalColumn: "Code");
            });

        migrationBuilder.CreateTable(
            name: "AirlineContact",
            schema: "metadata",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                AirlineCode = table.Column<string>(type: "citext", maxLength: 3, nullable: false),
                ContactType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                Value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AirlineContact", x => x.Id);
                table.ForeignKey(
                    name: "FK_AirlineContact_Airline_AirlineCode",
                    column: x => x.AirlineCode,
                    principalSchema: "metadata",
                    principalTable: "Airline",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AirlineLogo",
            schema: "metadata",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                AirlineCode = table.Column<string>(type: "citext", maxLength: 3, nullable: false),
                Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                ImageData = table.Column<byte[]>(type: "bytea", nullable: false),
                ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AirlineLogo", x => x.Id);
                table.ForeignKey(
                    name: "FK_AirlineLogo_Airline_AirlineCode",
                    column: x => x.AirlineCode,
                    principalSchema: "metadata",
                    principalTable: "Airline",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Gate",
            schema: "metadata",
            columns: table => new
            {
                Code = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                GateTypeCode = table.Column<string>(type: "citext", maxLength: 1, nullable: true),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                Capacity = table.Column<int>(type: "integer", nullable: true),
                AirlineCode = table.Column<string>(type: "citext", maxLength: 3, nullable: true),
                HasLoadingBridge = table.Column<bool>(type: "boolean", nullable: false),
                HasPca = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Gate", x => x.Code);
                table.ForeignKey(
                    name: "FK_Gate_Airline_AirlineCode",
                    column: x => x.AirlineCode,
                    principalSchema: "metadata",
                    principalTable: "Airline",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_Gate_GateType_GateTypeCode",
                    column: x => x.GateTypeCode,
                    principalSchema: "metadata",
                    principalTable: "GateType",
                    principalColumn: "Code");
            });

        migrationBuilder.CreateTable(
            name: "AirportImage",
            schema: "metadata",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                AirportCode = table.Column<string>(type: "citext", maxLength: 3, nullable: false),
                ImageData = table.Column<byte[]>(type: "bytea", nullable: false),
                ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AirportImage", x => x.Id);
                table.ForeignKey(
                    name: "FK_AirportImage_Airport_AirportCode",
                    column: x => x.AirportCode,
                    principalSchema: "metadata",
                    principalTable: "Airport",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "TransferPassenger",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                ArrivalFlightId = table.Column<long>(type: "bigint", nullable: true),
                DepartureFlightId = table.Column<long>(type: "bigint", nullable: true),
                ImportMessageId = table.Column<long>(type: "bigint", nullable: true),
                FlightElement = table.Column<string>(type: "text", nullable: true),
                ArrFlightNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                ArrDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DepFlightNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                DepDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DepDestination = table.Column<string>(type: "citext", maxLength: 3, nullable: true),
                NumberOfSeats = table.Column<int>(type: "integer", nullable: false),
                PassengerNames = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                BookingClass = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                NumberOfBags = table.Column<int>(type: "integer", nullable: false),
                BagWeight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TransferPassenger", x => x.Id);
                table.ForeignKey(
                    name: "FK_TransferPassenger_Airport_DepDestination",
                    column: x => x.DepDestination,
                    principalSchema: "metadata",
                    principalTable: "Airport",
                    principalColumn: "Code");
            });

        migrationBuilder.CreateTable(
            name: "ArrivalFlight",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"DepartureFlightSequence\"')"),
                AirlineCode = table.Column<string>(type: "citext", maxLength: 3, nullable: false),
                FlightNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                FlightDate = table.Column<DateOnly>(type: "date", nullable: false),
                CallSign = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                CodeshareNumber = table.Column<int>(type: "integer", nullable: true),
                FlightTypeCode = table.Column<string>(type: "citext", maxLength: 1, nullable: false),
                StatusCode = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                CountryType = table.Column<string>(type: "citext", maxLength: 1, nullable: true),
                AircraftCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                AircraftTypeCode = table.Column<string>(type: "citext", maxLength: 5, nullable: true),
                OperatingAirlineCode = table.Column<string>(type: "citext", maxLength: 3, nullable: true),
                AircraftOwner = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                ParkPositionCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                CurrentParkPositionCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                TerminalCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                HandlingAgentCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                Comment = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                IsGeneralAviation = table.Column<bool>(type: "boolean", nullable: false),
                CancellationDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                DepartureFlightId = table.Column<long>(type: "bigint", nullable: true),
                Sta = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Eta = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Ata = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                TouchDown = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                OnBlock = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                DivertAirportCode = table.Column<string>(type: "citext", maxLength: 3, nullable: true),
                IsDivertedFlight = table.Column<bool>(type: "boolean", nullable: false),
                BeltLastBag = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArrivalFlight", x => x.Id);
                table.ForeignKey(
                    name: "FK_ArrivalFlight_AircraftType_AircraftTypeCode",
                    column: x => x.AircraftTypeCode,
                    principalSchema: "metadata",
                    principalTable: "AircraftType",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_ArrivalFlight_Aircraft_AircraftCode",
                    column: x => x.AircraftCode,
                    principalSchema: "metadata",
                    principalTable: "Aircraft",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_ArrivalFlight_Airline_AirlineCode",
                    column: x => x.AirlineCode,
                    principalSchema: "metadata",
                    principalTable: "Airline",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ArrivalFlight_Airline_OperatingAirlineCode",
                    column: x => x.OperatingAirlineCode,
                    principalSchema: "metadata",
                    principalTable: "Airline",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_ArrivalFlight_Airport_DivertAirportCode",
                    column: x => x.DivertAirportCode,
                    principalSchema: "metadata",
                    principalTable: "Airport",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_ArrivalFlight_CountryType_CountryType",
                    column: x => x.CountryType,
                    principalSchema: "metadata",
                    principalTable: "CountryType",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_ArrivalFlight_FlightType_FlightTypeCode",
                    column: x => x.FlightTypeCode,
                    principalSchema: "metadata",
                    principalTable: "FlightType",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ArrivalFlight_ParkPosition_CurrentParkPositionCode",
                    column: x => x.CurrentParkPositionCode,
                    principalSchema: "metadata",
                    principalTable: "ParkPosition",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_ArrivalFlight_ParkPosition_ParkPositionCode",
                    column: x => x.ParkPositionCode,
                    principalSchema: "metadata",
                    principalTable: "ParkPosition",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_ArrivalFlight_Terminal_TerminalCode",
                    column: x => x.TerminalCode,
                    principalSchema: "metadata",
                    principalTable: "Terminal",
                    principalColumn: "Code");
            });

        migrationBuilder.CreateTable(
            name: "DepartureFlight",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"DepartureFlightSequence\"')"),
                AirlineCode = table.Column<string>(type: "citext", maxLength: 3, nullable: false),
                FlightNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                FlightDate = table.Column<DateOnly>(type: "date", nullable: false),
                CallSign = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                CodeshareNumber = table.Column<int>(type: "integer", nullable: true),
                FlightTypeCode = table.Column<string>(type: "citext", maxLength: 1, nullable: false),
                StatusCode = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                CountryType = table.Column<string>(type: "citext", maxLength: 1, nullable: true),
                AircraftCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                AircraftTypeCode = table.Column<string>(type: "citext", maxLength: 5, nullable: true),
                OperatingAirlineCode = table.Column<string>(type: "citext", maxLength: 3, nullable: true),
                AircraftOwner = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                ParkPositionCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                CurrentParkPositionCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                TerminalCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                HandlingAgentCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                Comment = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                IsGeneralAviation = table.Column<bool>(type: "boolean", nullable: false),
                CancellationDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                ArrivalFlightId = table.Column<long>(type: "bigint", nullable: true),
                Std = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Etd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Atd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                OffBlock = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                TakeOff = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                FinalCall = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DepartureFlight", x => x.Id);
                table.ForeignKey(
                    name: "FK_DepartureFlight_AircraftType_AircraftTypeCode",
                    column: x => x.AircraftTypeCode,
                    principalSchema: "metadata",
                    principalTable: "AircraftType",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_DepartureFlight_Aircraft_AircraftCode",
                    column: x => x.AircraftCode,
                    principalSchema: "metadata",
                    principalTable: "Aircraft",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_DepartureFlight_Airline_AirlineCode",
                    column: x => x.AirlineCode,
                    principalSchema: "metadata",
                    principalTable: "Airline",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_DepartureFlight_Airline_OperatingAirlineCode",
                    column: x => x.OperatingAirlineCode,
                    principalSchema: "metadata",
                    principalTable: "Airline",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_DepartureFlight_CountryType_CountryType",
                    column: x => x.CountryType,
                    principalSchema: "metadata",
                    principalTable: "CountryType",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_DepartureFlight_FlightType_FlightTypeCode",
                    column: x => x.FlightTypeCode,
                    principalSchema: "metadata",
                    principalTable: "FlightType",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_DepartureFlight_ParkPosition_CurrentParkPositionCode",
                    column: x => x.CurrentParkPositionCode,
                    principalSchema: "metadata",
                    principalTable: "ParkPosition",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_DepartureFlight_ParkPosition_ParkPositionCode",
                    column: x => x.ParkPositionCode,
                    principalSchema: "metadata",
                    principalTable: "ParkPosition",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_DepartureFlight_Terminal_TerminalCode",
                    column: x => x.TerminalCode,
                    principalSchema: "metadata",
                    principalTable: "Terminal",
                    principalColumn: "Code");
            });

        migrationBuilder.CreateTable(
            name: "ArrivalBelt",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                FlightId = table.Column<long>(type: "bigint", nullable: false),
                Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                StatusCode = table.Column<string>(type: "citext", maxLength: 5, nullable: true),
                BeginPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                EndPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                BeginActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                EndActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                IsComplimentary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                LastCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                DisplayCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                PayerCompanyCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                LastBag = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArrivalBelt", x => x.Id);
                table.ForeignKey(
                    name: "FK_ArrivalBelt_ArrivalBeltStatus_StatusCode",
                    column: x => x.StatusCode,
                    principalSchema: "metadata",
                    principalTable: "ArrivalBeltStatus",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_ArrivalBelt_ArrivalFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "ArrivalFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ArrivalCodeShare",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                AirlineCode = table.Column<string>(type: "citext", maxLength: 3, nullable: false),
                FlightNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArrivalCodeShare", x => x.Id);
                table.ForeignKey(
                    name: "FK_ArrivalCodeShare_Airline_AirlineCode",
                    column: x => x.AirlineCode,
                    principalSchema: "metadata",
                    principalTable: "Airline",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ArrivalCodeShare_ArrivalFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "ArrivalFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ArrivalDelay",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                DelayCode = table.Column<string>(type: "text", nullable: false),
                DelayMinutes = table.Column<decimal>(type: "numeric", nullable: true),
                Comment = table.Column<string>(type: "text", nullable: true),
                FlightId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArrivalDelay", x => x.Id);
                table.ForeignKey(
                    name: "FK_ArrivalDelay_ArrivalFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "ArrivalFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ArrivalFlightLoadInfo",
            schema: "ops",
            columns: table => new
            {
                FlightId = table.Column<long>(type: "bigint", nullable: false),
                PaxTotal = table.Column<int>(type: "integer", nullable: true),
                PaxTransit = table.Column<int>(type: "integer", nullable: true),
                PaxAssistanceRequired = table.Column<int>(type: "integer", nullable: true),
                Pax1 = table.Column<int>(type: "integer", nullable: true),
                Pax2 = table.Column<int>(type: "integer", nullable: true),
                Pax3 = table.Column<int>(type: "integer", nullable: true),
                CrewNumber = table.Column<int>(type: "integer", nullable: true),
                CrewCockpit = table.Column<int>(type: "integer", nullable: true),
                BaggageWeight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                BaggageTotal = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                CargoWeight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                MailWeight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                TotalWeight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                EquipmentWeight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                CabinLuggage = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArrivalFlightLoadInfo", x => x.FlightId);
                table.ForeignKey(
                    name: "FK_ArrivalFlightLoadInfo_ArrivalFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "ArrivalFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ArrivalGate",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                FlightId = table.Column<long>(type: "bigint", nullable: false),
                Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                StatusCode = table.Column<string>(type: "citext", maxLength: 5, nullable: true),
                BeginPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                EndPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                BeginActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                EndActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                IsComplimentary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                LastCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                DisplayCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                PayerCompanyCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArrivalGate", x => x.Id);
                table.ForeignKey(
                    name: "FK_ArrivalGate_ArrivalFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "ArrivalFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ArrivalGate_GateStatus_StatusCode",
                    column: x => x.StatusCode,
                    principalSchema: "metadata",
                    principalTable: "GateStatus",
                    principalColumn: "Code");
            });

        migrationBuilder.CreateTable(
            name: "ArrivalPassenger",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                PassengerType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                Count = table.Column<int>(type: "integer", nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArrivalPassenger", x => x.Id);
                table.ForeignKey(
                    name: "FK_ArrivalPassenger_ArrivalFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "ArrivalFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ArrivalRouting",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"DepartureRoutingSequence\"')"),
                Code = table.Column<string>(type: "text", nullable: false),
                LegNumber = table.Column<int>(type: "integer", nullable: false),
                Sta = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Std = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                OnBlock = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                OffBlock = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                FlightId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArrivalRouting", x => x.Id);
                table.ForeignKey(
                    name: "FK_ArrivalRouting_ArrivalFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "ArrivalFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ArrivalService",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                ServiceCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                AmountPlan = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                AmountActual = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                BeginPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                EndPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                BeginActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                EndActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                HandlingAgentCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                PayerCompanyCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                FlightId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArrivalService", x => x.Id);
                table.ForeignKey(
                    name: "FK_ArrivalService_ArrivalFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "ArrivalFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "DepartureBelt",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                FlightId = table.Column<long>(type: "bigint", nullable: false),
                Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                StatusCode = table.Column<string>(type: "citext", maxLength: 5, nullable: true),
                BeginPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                EndPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                BeginActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                EndActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                IsComplimentary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                LastCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                DisplayCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                PayerCompanyCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DepartureBelt", x => x.Id);
                table.ForeignKey(
                    name: "FK_DepartureBelt_DepartureBeltStatus_StatusCode",
                    column: x => x.StatusCode,
                    principalSchema: "metadata",
                    principalTable: "DepartureBeltStatus",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_DepartureBelt_DepartureFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "DepartureFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "DepartureCodeShare",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                AirlineCode = table.Column<string>(type: "citext", maxLength: 3, nullable: false),
                FlightNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DepartureCodeShare", x => x.Id);
                table.ForeignKey(
                    name: "FK_DepartureCodeShare_Airline_AirlineCode",
                    column: x => x.AirlineCode,
                    principalSchema: "metadata",
                    principalTable: "Airline",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_DepartureCodeShare_DepartureFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "DepartureFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "DepartureCounter",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                FlightId = table.Column<long>(type: "bigint", nullable: false),
                Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                StatusCode = table.Column<string>(type: "citext", maxLength: 5, nullable: true),
                BeginPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                EndPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                BeginActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                EndActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                IsComplimentary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                LastCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                DisplayCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                PayerCompanyCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                CheckinClassCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DepartureCounter", x => x.Id);
                table.ForeignKey(
                    name: "FK_DepartureCounter_CounterStatus_StatusCode",
                    column: x => x.StatusCode,
                    principalSchema: "metadata",
                    principalTable: "CounterStatus",
                    principalColumn: "Code");
                table.ForeignKey(
                    name: "FK_DepartureCounter_DepartureFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "DepartureFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "DepartureDelay",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                DelayCode = table.Column<string>(type: "text", nullable: false),
                DelayMinutes = table.Column<decimal>(type: "numeric", nullable: true),
                Comment = table.Column<string>(type: "text", nullable: true),
                FlightId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DepartureDelay", x => x.Id);
                table.ForeignKey(
                    name: "FK_DepartureDelay_DepartureFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "DepartureFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "DepartureFlightLoadInfo",
            schema: "ops",
            columns: table => new
            {
                FlightId = table.Column<long>(type: "bigint", nullable: false),
                PaxTotal = table.Column<int>(type: "integer", nullable: true),
                PaxTransit = table.Column<int>(type: "integer", nullable: true),
                PaxAssistanceRequired = table.Column<int>(type: "integer", nullable: true),
                Pax1 = table.Column<int>(type: "integer", nullable: true),
                Pax2 = table.Column<int>(type: "integer", nullable: true),
                Pax3 = table.Column<int>(type: "integer", nullable: true),
                CrewNumber = table.Column<int>(type: "integer", nullable: true),
                CrewCockpit = table.Column<int>(type: "integer", nullable: true),
                BaggageWeight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                BaggageTotal = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                CargoWeight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                MailWeight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                TotalWeight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                EquipmentWeight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                CabinLuggage = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DepartureFlightLoadInfo", x => x.FlightId);
                table.ForeignKey(
                    name: "FK_DepartureFlightLoadInfo_DepartureFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "DepartureFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "DepartureGate",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                FlightId = table.Column<long>(type: "bigint", nullable: false),
                Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                StatusCode = table.Column<string>(type: "citext", maxLength: 5, nullable: true),
                BeginPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                EndPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                BeginActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                EndActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                IsComplimentary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                LastCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                DisplayCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                PayerCompanyCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                BoardingPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                BoardingActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DepartureGate", x => x.Id);
                table.ForeignKey(
                    name: "FK_DepartureGate_DepartureFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "DepartureFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_DepartureGate_GateStatus_StatusCode",
                    column: x => x.StatusCode,
                    principalSchema: "metadata",
                    principalTable: "GateStatus",
                    principalColumn: "Code");
            });

        migrationBuilder.CreateTable(
            name: "DeparturePassenger",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                PassengerType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                Count = table.Column<int>(type: "integer", nullable: false),
                FlightId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DeparturePassenger", x => x.Id);
                table.ForeignKey(
                    name: "FK_DeparturePassenger_DepartureFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "DepartureFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "DepartureRouting",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"DepartureRoutingSequence\"')"),
                Code = table.Column<string>(type: "text", nullable: false),
                LegNumber = table.Column<int>(type: "integer", nullable: false),
                Sta = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Std = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                OnBlock = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                OffBlock = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                FlightId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DepartureRouting", x => x.Id);
                table.ForeignKey(
                    name: "FK_DepartureRouting_DepartureFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "DepartureFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "DepartureService",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                ServiceCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                AmountPlan = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                AmountActual = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                BeginPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                EndPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                BeginActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                EndActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                HandlingAgentCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                PayerCompanyCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                FlightId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DepartureService", x => x.Id);
                table.ForeignKey(
                    name: "FK_DepartureService_DepartureFlight_FlightId",
                    column: x => x.FlightId,
                    principalSchema: "ops",
                    principalTable: "DepartureFlight",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ParkPosition",
            schema: "ops",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"FlightResourceSequence\"')"),
                ArrivalFlightId = table.Column<long>(type: "bigint", nullable: true),
                DepartureFlightId = table.Column<long>(type: "bigint", nullable: true),
                Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                LastCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                Order = table.Column<int>(type: "integer", nullable: false),
                BeginPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                EndPlan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                BeginActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                EndActual = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                TowCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                TowIndicator = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                HandlingAgentCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                PayerCompanyCode = table.Column<string>(type: "citext", maxLength: 10, nullable: true),
                FlightStatusCode = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ParkPosition1", x => x.Id);
                table.ForeignKey(
                    name: "FK_ParkPosition_ArrivalFlight_ArrivalFlightId",
                    column: x => x.ArrivalFlightId,
                    principalSchema: "ops",
                    principalTable: "ArrivalFlight",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_ParkPosition_DepartureFlight_DepartureFlightId",
                    column: x => x.DepartureFlightId,
                    principalSchema: "ops",
                    principalTable: "DepartureFlight",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_AeroConfigurationAudit_Action_Timestamp",
            schema: "audits",
            table: "AeroConfigurationAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_Aircraft_AircraftTypeCode",
            schema: "metadata",
            table: "Aircraft",
            column: "AircraftTypeCode");

        migrationBuilder.CreateIndex(
            name: "IX_Aircraft_AirlineCode",
            schema: "metadata",
            table: "Aircraft",
            column: "AirlineCode");

        migrationBuilder.CreateIndex(
            name: "IX_Aircraft_NoiseCategory",
            schema: "metadata",
            table: "Aircraft",
            column: "NoiseCategory");

        migrationBuilder.CreateIndex(
            name: "IX_Aircraft_SizeCategory",
            schema: "metadata",
            table: "Aircraft",
            column: "SizeCategory");

        migrationBuilder.CreateIndex(
            name: "IX_AircraftAudit_Action_Timestamp",
            schema: "audits",
            table: "AircraftAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_AircraftSizeCategoryAudit_Action_Timestamp",
            schema: "audits",
            table: "AircraftSizeCategoryAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_AircraftType_NoiseCategory",
            schema: "metadata",
            table: "AircraftType",
            column: "NoiseCategory");

        migrationBuilder.CreateIndex(
            name: "IX_AircraftType_SizeCategory",
            schema: "metadata",
            table: "AircraftType",
            column: "SizeCategory");

        migrationBuilder.CreateIndex(
            name: "IX_AircraftTypeAudit_Action_Timestamp",
            schema: "audits",
            table: "AircraftTypeAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_Airline_CountryCode",
            schema: "metadata",
            table: "Airline",
            column: "CountryCode");

        migrationBuilder.CreateIndex(
            name: "IX_Airline_ParentAirlineCode",
            schema: "metadata",
            table: "Airline",
            column: "ParentAirlineCode");

        migrationBuilder.CreateIndex(
            name: "IX_AirlineAudit_Action_Timestamp",
            schema: "audits",
            table: "AirlineAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_AirlineContact_AirlineCode",
            schema: "metadata",
            table: "AirlineContact",
            column: "AirlineCode");

        migrationBuilder.CreateIndex(
            name: "IX_AirlineLogo_AirlineCode",
            schema: "metadata",
            table: "AirlineLogo",
            column: "AirlineCode");

        migrationBuilder.CreateIndex(
            name: "IX_Airport_CountryCode",
            schema: "metadata",
            table: "Airport",
            column: "CountryCode");

        migrationBuilder.CreateIndex(
            name: "IX_Airport_TimezoneCode",
            schema: "metadata",
            table: "Airport",
            column: "TimezoneCode");

        migrationBuilder.CreateIndex(
            name: "IX_AirportAudit_Action_Timestamp",
            schema: "audits",
            table: "AirportAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_AirportImage_AirportCode",
            schema: "metadata",
            table: "AirportImage",
            column: "AirportCode");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalBelt_Code_BeginPlan_EndPlan",
            schema: "ops",
            table: "ArrivalBelt",
            columns: new[] { "Code", "BeginPlan", "EndPlan" });

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalBelt_FlightId",
            schema: "ops",
            table: "ArrivalBelt",
            column: "FlightId");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalBelt_StatusCode",
            schema: "ops",
            table: "ArrivalBelt",
            column: "StatusCode");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalBeltAudit_Action_Timestamp",
            schema: "audits",
            table: "ArrivalBeltAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalCodeShare_AirlineCode",
            schema: "ops",
            table: "ArrivalCodeShare",
            column: "AirlineCode");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalCodeShare_FlightId",
            schema: "ops",
            table: "ArrivalCodeShare",
            column: "FlightId");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalDelay_FlightId",
            schema: "ops",
            table: "ArrivalDelay",
            column: "FlightId");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalFlight_AircraftCode",
            schema: "ops",
            table: "ArrivalFlight",
            column: "AircraftCode");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalFlight_AircraftTypeCode",
            schema: "ops",
            table: "ArrivalFlight",
            column: "AircraftTypeCode");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalFlight_AirlineCode",
            schema: "ops",
            table: "ArrivalFlight",
            column: "AirlineCode");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalFlight_AirlineCode_FlightNumber_FlightDate",
            schema: "ops",
            table: "ArrivalFlight",
            columns: new[] { "AirlineCode", "FlightNumber", "FlightDate" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalFlight_CountryType",
            schema: "ops",
            table: "ArrivalFlight",
            column: "CountryType");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalFlight_CurrentParkPositionCode",
            schema: "ops",
            table: "ArrivalFlight",
            column: "CurrentParkPositionCode");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalFlight_DivertAirportCode",
            schema: "ops",
            table: "ArrivalFlight",
            column: "DivertAirportCode");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalFlight_FlightTypeCode",
            schema: "ops",
            table: "ArrivalFlight",
            column: "FlightTypeCode");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalFlight_OperatingAirlineCode",
            schema: "ops",
            table: "ArrivalFlight",
            column: "OperatingAirlineCode");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalFlight_ParkPositionCode",
            schema: "ops",
            table: "ArrivalFlight",
            column: "ParkPositionCode");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalFlight_TerminalCode",
            schema: "ops",
            table: "ArrivalFlight",
            column: "TerminalCode");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalFlightAudit_Action_Timestamp",
            schema: "audits",
            table: "ArrivalFlightAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalGate_Code_BeginPlan_EndPlan",
            schema: "ops",
            table: "ArrivalGate",
            columns: new[] { "Code", "BeginPlan", "EndPlan" });

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalGate_FlightId",
            schema: "ops",
            table: "ArrivalGate",
            column: "FlightId");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalGate_StatusCode",
            schema: "ops",
            table: "ArrivalGate",
            column: "StatusCode");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalPassenger_FlightId",
            schema: "ops",
            table: "ArrivalPassenger",
            column: "FlightId");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalRouting_FlightId",
            schema: "ops",
            table: "ArrivalRouting",
            column: "FlightId");

        migrationBuilder.CreateIndex(
            name: "IX_ArrivalService_FlightId",
            schema: "ops",
            table: "ArrivalService",
            column: "FlightId");

        migrationBuilder.CreateIndex(
            name: "IX_CheckinClassAudit_Action_Timestamp",
            schema: "audits",
            table: "CheckinClassAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_Company_CompanyTypeCode",
            schema: "metadata",
            table: "Company",
            column: "CompanyTypeCode");

        migrationBuilder.CreateIndex(
            name: "IX_CompanyAudit_Action_Timestamp",
            schema: "audits",
            table: "CompanyAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_CompanyContact_CompanyCode",
            schema: "metadata",
            table: "CompanyContact",
            column: "CompanyCode");

        migrationBuilder.CreateIndex(
            name: "IX_CompanyTypeAudit_Action_Timestamp",
            schema: "audits",
            table: "CompanyTypeAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_CounterAudit_Action_Timestamp",
            schema: "audits",
            table: "CounterAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_Country_CountryTypeCode",
            schema: "metadata",
            table: "Country",
            column: "CountryTypeCode");

        migrationBuilder.CreateIndex(
            name: "IX_Country_RegionCode",
            schema: "metadata",
            table: "Country",
            column: "RegionCode");

        migrationBuilder.CreateIndex(
            name: "IX_CountryAudit_Action_Timestamp",
            schema: "audits",
            table: "CountryAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_DelayReasonAudit_Action_Timestamp",
            schema: "audits",
            table: "DelayReasonAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_DepartureBelt_Code_BeginPlan_EndPlan",
            schema: "ops",
            table: "DepartureBelt",
            columns: new[] { "Code", "BeginPlan", "EndPlan" });

        migrationBuilder.CreateIndex(
            name: "IX_DepartureBelt_FlightId",
            schema: "ops",
            table: "DepartureBelt",
            column: "FlightId");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureBelt_StatusCode",
            schema: "ops",
            table: "DepartureBelt",
            column: "StatusCode");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureBeltAudit_Action_Timestamp",
            schema: "audits",
            table: "DepartureBeltAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_DepartureCodeShare_AirlineCode",
            schema: "ops",
            table: "DepartureCodeShare",
            column: "AirlineCode");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureCodeShare_FlightId",
            schema: "ops",
            table: "DepartureCodeShare",
            column: "FlightId");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureCounter_Code_BeginPlan_EndPlan",
            schema: "ops",
            table: "DepartureCounter",
            columns: new[] { "Code", "BeginPlan", "EndPlan" });

        migrationBuilder.CreateIndex(
            name: "IX_DepartureCounter_FlightId",
            schema: "ops",
            table: "DepartureCounter",
            column: "FlightId");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureCounter_StatusCode",
            schema: "ops",
            table: "DepartureCounter",
            column: "StatusCode");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureDelay_FlightId",
            schema: "ops",
            table: "DepartureDelay",
            column: "FlightId");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureFlight_AircraftCode",
            schema: "ops",
            table: "DepartureFlight",
            column: "AircraftCode");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureFlight_AircraftTypeCode",
            schema: "ops",
            table: "DepartureFlight",
            column: "AircraftTypeCode");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureFlight_AirlineCode",
            schema: "ops",
            table: "DepartureFlight",
            column: "AirlineCode");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureFlight_AirlineCode_FlightNumber_FlightDate",
            schema: "ops",
            table: "DepartureFlight",
            columns: new[] { "AirlineCode", "FlightNumber", "FlightDate" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_DepartureFlight_CountryType",
            schema: "ops",
            table: "DepartureFlight",
            column: "CountryType");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureFlight_CurrentParkPositionCode",
            schema: "ops",
            table: "DepartureFlight",
            column: "CurrentParkPositionCode");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureFlight_FlightTypeCode",
            schema: "ops",
            table: "DepartureFlight",
            column: "FlightTypeCode");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureFlight_OperatingAirlineCode",
            schema: "ops",
            table: "DepartureFlight",
            column: "OperatingAirlineCode");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureFlight_ParkPositionCode",
            schema: "ops",
            table: "DepartureFlight",
            column: "ParkPositionCode");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureFlight_TerminalCode",
            schema: "ops",
            table: "DepartureFlight",
            column: "TerminalCode");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureFlightAudit_Action_Timestamp",
            schema: "audits",
            table: "DepartureFlightAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_DepartureGate_Code_BeginPlan_EndPlan",
            schema: "ops",
            table: "DepartureGate",
            columns: new[] { "Code", "BeginPlan", "EndPlan" });

        migrationBuilder.CreateIndex(
            name: "IX_DepartureGate_FlightId",
            schema: "ops",
            table: "DepartureGate",
            column: "FlightId");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureGate_StatusCode",
            schema: "ops",
            table: "DepartureGate",
            column: "StatusCode");

        migrationBuilder.CreateIndex(
            name: "IX_DeparturePassenger_FlightId",
            schema: "ops",
            table: "DeparturePassenger",
            column: "FlightId");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureRouting_FlightId",
            schema: "ops",
            table: "DepartureRouting",
            column: "FlightId");

        migrationBuilder.CreateIndex(
            name: "IX_DepartureService_FlightId",
            schema: "ops",
            table: "DepartureService",
            column: "FlightId");

        migrationBuilder.CreateIndex(
            name: "IX_Gate_AirlineCode",
            schema: "metadata",
            table: "Gate",
            column: "AirlineCode");

        migrationBuilder.CreateIndex(
            name: "IX_Gate_GateTypeCode",
            schema: "metadata",
            table: "Gate",
            column: "GateTypeCode");

        migrationBuilder.CreateIndex(
            name: "IX_GateAudit_Action_Timestamp",
            schema: "audits",
            table: "GateAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_HandlingServiceAudit_Action_Timestamp",
            schema: "audits",
            table: "HandlingServiceAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_InboxMessage_ProcessedAt",
            schema: "ops",
            table: "InboxMessage",
            column: "ProcessedAt");

        migrationBuilder.CreateIndex(
            name: "IX_InboxMessage_Unprocessed",
            schema: "ops",
            table: "InboxMessage",
            column: "Id",
            filter: "\"ProcessedAt\" IS NULL");

        migrationBuilder.CreateIndex(
            name: "IX_OpsArrivalBeltAudit_Action_Timestamp",
            schema: "audits",
            table: "OpsArrivalBeltAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_OpsArrivalGateAudit_Action_Timestamp",
            schema: "audits",
            table: "OpsArrivalGateAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_OpsCounterAudit_Action_Timestamp",
            schema: "audits",
            table: "OpsCounterAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_OpsDepartureBeltAudit_Action_Timestamp",
            schema: "audits",
            table: "OpsDepartureBeltAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_OpsDepartureGateAudit_Action_Timestamp",
            schema: "audits",
            table: "OpsDepartureGateAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_OpsFlightParkPositionAudit_Action_Timestamp",
            schema: "audits",
            table: "OpsFlightParkPositionAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessage_PublishedAt",
            schema: "ops",
            table: "OutboxMessage",
            column: "PublishedAt");

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessage_Unpublished",
            schema: "ops",
            table: "OutboxMessage",
            column: "Id",
            filter: "\"PublishedAt\" IS NULL");

        migrationBuilder.CreateIndex(
            name: "IX_ParkPosition_TypeCode",
            schema: "metadata",
            table: "ParkPosition",
            column: "TypeCode");

        migrationBuilder.CreateIndex(
            name: "IX_ParkPosition_ArrivalFlightId",
            schema: "ops",
            table: "ParkPosition",
            column: "ArrivalFlightId");

        migrationBuilder.CreateIndex(
            name: "IX_ParkPosition_Code_BeginPlan_EndPlan",
            schema: "ops",
            table: "ParkPosition",
            columns: new[] { "Code", "BeginPlan", "EndPlan" });

        migrationBuilder.CreateIndex(
            name: "IX_ParkPosition_DepartureFlightId",
            schema: "ops",
            table: "ParkPosition",
            column: "DepartureFlightId");

        migrationBuilder.CreateIndex(
            name: "IX_ParkPositionAudit_Action_Timestamp",
            schema: "audits",
            table: "ParkPositionAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_RemarkAudit_Action_Timestamp",
            schema: "audits",
            table: "RemarkAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_Role_Name",
            schema: "identity",
            table: "Role",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_RoleAudit_Action_Timestamp",
            schema: "audits",
            table: "RoleAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_RolePermission_PermissionCode",
            schema: "identity",
            table: "RolePermission",
            column: "PermissionCode");

        migrationBuilder.CreateIndex(
            name: "IX_Rule_IsEnabled",
            schema: "ops",
            table: "Rule",
            column: "IsEnabled");

        migrationBuilder.CreateIndex(
            name: "IX_RuleAction_RuleId",
            schema: "ops",
            table: "RuleAction",
            column: "RuleId");

        migrationBuilder.CreateIndex(
            name: "IX_RuleCondition_GroupId",
            schema: "ops",
            table: "RuleCondition",
            column: "GroupId");

        migrationBuilder.CreateIndex(
            name: "IX_RuleConditionGroup_RuleId",
            schema: "ops",
            table: "RuleConditionGroup",
            column: "RuleId");

        migrationBuilder.CreateIndex(
            name: "IX_RuleTrigger_RuleId_Trigger",
            schema: "ops",
            table: "RuleTrigger",
            columns: new[] { "RuleId", "Trigger" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_RunwayAudit_Action_Timestamp",
            schema: "audits",
            table: "RunwayAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_SeasonalFlightScheduleArrivalFlight_ScheduleSeasonCode",
            schema: "ops",
            table: "SeasonalFlightScheduleArrivalFlight",
            column: "ScheduleSeasonCode");

        migrationBuilder.CreateIndex(
            name: "IX_SeasonalFlightScheduleArrivalFlightRouting_SeasonalFlightSc~",
            schema: "ops",
            table: "SeasonalFlightScheduleArrivalFlightRouting",
            column: "SeasonalFlightScheduleFlightId");

        migrationBuilder.CreateIndex(
            name: "IX_SeasonalFlightScheduleDepartureFlight_ScheduleSeasonCode",
            schema: "ops",
            table: "SeasonalFlightScheduleDepartureFlight",
            column: "ScheduleSeasonCode");

        migrationBuilder.CreateIndex(
            name: "IX_SeasonalFlightScheduleDepartureFlightRouting_SeasonalFlight~",
            schema: "ops",
            table: "SeasonalFlightScheduleDepartureFlightRouting",
            column: "SeasonalFlightScheduleFlightId");

        migrationBuilder.CreateIndex(
            name: "IX_SystemProperty_Code",
            schema: "ops",
            table: "SystemProperty",
            column: "Code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_TerminalAudit_Action_Timestamp",
            schema: "audits",
            table: "TerminalAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_TransferPassenger_DepDestination",
            schema: "ops",
            table: "TransferPassenger",
            column: "DepDestination");

        migrationBuilder.CreateIndex(
            name: "IX_User_Username",
            schema: "identity",
            table: "User",
            column: "Username",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_UserAudit_Action_Timestamp",
            schema: "audits",
            table: "UserAudit",
            columns: new[] { "Action", "Timestamp" });

        migrationBuilder.CreateIndex(
            name: "IX_UserRole_RoleId",
            schema: "identity",
            table: "UserRole",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "IX_UserRole_UserId_RoleId",
            schema: "identity",
            table: "UserRole",
            columns: new[] { "UserId", "RoleId" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AeroConfiguration",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "AeroConfigurationAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "AircraftAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "AircraftSizeCategoryAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "AircraftTypeAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "AirlineAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "AirlineContact",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "AirlineLogo",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "AirportAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "AirportImage",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "ArrivalBelt",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "ArrivalBelt",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "ArrivalBeltAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "ArrivalCodeShare",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "ArrivalDelay",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "ArrivalFlightAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "ArrivalFlightLoadInfo",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "ArrivalGate",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "ArrivalMovementMessage",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "ArrivalPassenger",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "ArrivalRouting",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "ArrivalService",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "CheckinClass",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "CheckinClassAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "CompanyAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "CompanyContact",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "CompanyTypeAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "Counter",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "CounterAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "CountryAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "DeBoardingStatus",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "DelayReason",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "DelayReasonAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "DepartureBelt",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "DepartureBelt",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "DepartureBeltAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "DepartureCodeShare",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "DepartureCounter",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "DepartureDelay",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "DepartureFlightAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "DepartureFlightLoadInfo",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "DepartureGate",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "DepartureMovementMessage",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "DeparturePassenger",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "DepartureRouting",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "DepartureService",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "FlightStatus",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "Gate",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "GateAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "HandlingService",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "HandlingServiceAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "InboxMessage",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "JobSchedule",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "OpsArrivalBeltAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "OpsArrivalGateAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "OpsCounterAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "OpsDepartureBeltAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "OpsDepartureGateAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "OpsFlightParkPositionAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "OutboxMessage",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "ParkPosition",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "ParkPositionAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "Remark",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "RemarkAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "RoleAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "RolePermission",
            schema: "identity");

        migrationBuilder.DropTable(
            name: "RuleAction",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "RuleActionType",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "RuleCondition",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "RuleField",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "RuleFieldActionType",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "RuleFieldTrigger",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "RuleTrigger",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "RuleTriggerOption",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "Runway",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "RunwayAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "SeasonalFlightScheduleArrivalFlightRouting",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "SeasonalFlightScheduleDepartureFlightRouting",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "SystemProperty",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "TerminalAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "TransferPassenger",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "UserAudit",
            schema: "audits");

        migrationBuilder.DropTable(
            name: "UserRole",
            schema: "identity");

        migrationBuilder.DropTable(
            name: "ArrivalBeltStatus",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "Company",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "DepartureBeltStatus",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "CounterStatus",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "GateStatus",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "GateType",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "ArrivalFlight",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "DepartureFlight",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "Permission",
            schema: "identity");

        migrationBuilder.DropTable(
            name: "RuleConditionGroup",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "SeasonalFlightScheduleArrivalFlight",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "SeasonalFlightScheduleDepartureFlight",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "Role",
            schema: "identity");

        migrationBuilder.DropTable(
            name: "User",
            schema: "identity");

        migrationBuilder.DropTable(
            name: "CompanyType",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "Airport",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "Aircraft",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "FlightType",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "ParkPosition",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "Terminal",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "Rule",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "ScheduleSeason",
            schema: "ops");

        migrationBuilder.DropTable(
            name: "Timezone",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "AircraftType",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "Airline",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "ParkPositionType",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "AircraftNoiseCategory",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "AircraftSizeCategory",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "Country",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "CountryType",
            schema: "metadata");

        migrationBuilder.DropTable(
            name: "Region",
            schema: "metadata");

        migrationBuilder.DropSequence(
            name: "DepartureFlightSequence");

        migrationBuilder.DropSequence(
            name: "DepartureRoutingSequence");

        migrationBuilder.DropSequence(
            name: "FlightResourceSequence");

        migrationBuilder.DropSequence(
            name: "SeasonalFlightScheduleDepartureFlightRoutingSequence");

        migrationBuilder.DropSequence(
            name: "SeasonalFlightScheduleDepartureFlightSequence");
    }
}
