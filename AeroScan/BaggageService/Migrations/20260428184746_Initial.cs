using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BaggageService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "references");

            migrationBuilder.EnsureSchema(
                name: "audits");

            migrationBuilder.EnsureSchema(
                name: "flights");

            migrationBuilder.EnsureSchema(
                name: "bags");

            migrationBuilder.EnsureSchema(
                name: "messages");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.CreateTable(
                name: "AirlineClassMap",
                schema: "references",
                columns: table => new
                {
                    AirlineCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    SourceClass = table.Column<char>(type: "character(1)", maxLength: 1, nullable: false),
                    TargetClass = table.Column<char>(type: "character(1)", maxLength: 1, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineClassMap", x => new { x.AirlineCode, x.SourceClass });
                });

            migrationBuilder.CreateTable(
                name: "AirlineClassMapLog",
                schema: "audits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineClassMapLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AirlineHandlingContractFlightNumberLog",
                schema: "audits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineHandlingContractFlightNumberLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AirlineHandlingContractLog",
                schema: "audits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineHandlingContractLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArrivalFlightLog",
                schema: "audits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArrivalFlightLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "references",
                columns: table => new
                {
                    Code = table.Column<string>(type: "citext", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 150, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "CompanyLog",
                schema: "audits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContainerLog",
                schema: "audits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContainerTypeClasses",
                schema: "references",
                columns: table => new
                {
                    TypeCode = table.Column<string>(type: "citext", maxLength: 1, nullable: false),
                    ClassCode = table.Column<string>(type: "citext", maxLength: 1, nullable: false),
                    Description = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerTypeClasses", x => new { x.TypeCode, x.ClassCode });
                });

            migrationBuilder.CreateTable(
                name: "ContainerTypeClassLog",
                schema: "audits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerTypeClassLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContainerTypeLog",
                schema: "audits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerTypeLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContainerTypes",
                schema: "references",
                columns: table => new
                {
                    Code = table.Column<string>(type: "citext", maxLength: 1, nullable: false),
                    Description = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsAllDestination = table.Column<bool>(type: "boolean", nullable: false),
                    IsTransfer = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerTypes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "DepartureFlightContainers",
                schema: "flights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FlightId = table.Column<int>(type: "integer", nullable: false),
                    ContainerCode = table.Column<string>(type: "text", nullable: false),
                    ContainerTypeCode = table.Column<string>(type: "text", nullable: false),
                    ContainerStatusCode = table.Column<string>(type: "text", nullable: false),
                    ContainerClassCode = table.Column<string>(type: "text", nullable: false),
                    ContainerDestination = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartureFlightContainers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepartureFlightLog",
                schema: "audits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartureFlightLog", x => x.Id);
                });

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
                name: "HandheldTerminalLog",
                schema: "audits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandheldTerminalLog", x => x.Id);
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
                name: "PermissionLog",
                schema: "audits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "references",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Group = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReconciliationRecordSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TotalChecked = table.Column<int>(type: "integer", nullable: false),
                    TotalLoaded = table.Column<int>(type: "integer", nullable: false),
                    TotalOffloaded = table.Column<int>(type: "integer", nullable: false),
                    TotalTransferred = table.Column<int>(type: "integer", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClosedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReconciliationRecordSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceMaps",
                schema: "references",
                columns: table => new
                {
                    SourceResourceName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SourceResourceStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TargetResourceStatus = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceMaps", x => new { x.SourceResourceName, x.SourceResourceStatus });
                });

            migrationBuilder.CreateTable(
                name: "ResourceStatusMapLog",
                schema: "audits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceStatusMapLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleLog",
                schema: "audits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "references",
                columns: table => new
                {
                    Name = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfigurationLog",
                schema: "audits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfigurationLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfigurations",
                schema: "references",
                columns: table => new
                {
                    Key = table.Column<string>(type: "citext", maxLength: 25, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfigurations", x => x.Key);
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

            migrationBuilder.CreateTable(
                name: "UserLog",
                schema: "audits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimaryKey = table.Column<string>(type: "jsonb", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AirlineHandlingContracts",
                schema: "references",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AirlineCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    HandlingCompanyCode = table.Column<string>(type: "citext", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineHandlingContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirlineHandlingContracts_Companies_HandlingCompanyCode",
                        column: x => x.HandlingCompanyCode,
                        principalSchema: "references",
                        principalTable: "Companies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArrivalFlights",
                schema: "flights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OriginAirport = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    PreviousAirport = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Carousel = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    FlightStatus = table.Column<short>(type: "smallint", nullable: false),
                    RemoteSystemId = table.Column<string>(type: "citext", maxLength: 25, nullable: false),
                    AirlineCode = table.Column<string>(type: "citext", maxLength: 5, nullable: false),
                    FlightNumber = table.Column<string>(type: "citext", maxLength: 7, nullable: false),
                    ScheduledDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EstimatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FlightIataDate = table.Column<string>(type: "citext", maxLength: 5, nullable: false),
                    IntDom = table.Column<string>(type: "text", nullable: false),
                    Terminal = table.Column<string>(type: "text", nullable: true),
                    Registration = table.Column<string>(type: "text", nullable: true),
                    ParkingPosition = table.Column<string>(type: "text", nullable: true),
                    TotalPassengers = table.Column<int>(type: "integer", nullable: false),
                    HandlingCompanyCode = table.Column<string>(type: "citext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArrivalFlights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArrivalFlights_Companies_HandlingCompanyCode",
                        column: x => x.HandlingCompanyCode,
                        principalSchema: "references",
                        principalTable: "Companies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DepartureFlights",
                schema: "flights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DestinationAirport = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    NextAirport = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Gate = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    GateStatus = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    CheckIn = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CheckInStatus = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Chute = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    FlightStatus = table.Column<short>(type: "smallint", nullable: false),
                    RemoteSystemId = table.Column<string>(type: "citext", maxLength: 25, nullable: false),
                    AirlineCode = table.Column<string>(type: "citext", maxLength: 6, nullable: false),
                    FlightNumber = table.Column<string>(type: "citext", maxLength: 5, nullable: false),
                    ScheduledDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EstimatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FlightIataDate = table.Column<string>(type: "citext", maxLength: 5, nullable: false),
                    IntDom = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    Terminal = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Registration = table.Column<string>(type: "text", nullable: true),
                    ParkingPosition = table.Column<string>(type: "text", nullable: true),
                    TotalPassengers = table.Column<int>(type: "integer", nullable: false),
                    HandlingCompanyCode = table.Column<string>(type: "citext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartureFlights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartureFlights_Companies_HandlingCompanyCode",
                        column: x => x.HandlingCompanyCode,
                        principalSchema: "references",
                        principalTable: "Companies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Hhts",
                schema: "references",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceId = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    SerialNumber = table.Column<string>(type: "citext", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AssignedCompanyCode = table.Column<string>(type: "citext", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hhts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hhts_Companies_AssignedCompanyCode",
                        column: x => x.AssignedCompanyCode,
                        principalSchema: "references",
                        principalTable: "Companies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "references",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyCode = table.Column<string>(type: "citext", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Companies_CompanyCode",
                        column: x => x.CompanyCode,
                        principalSchema: "references",
                        principalTable: "Companies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                schema: "references",
                columns: table => new
                {
                    RoleName = table.Column<string>(type: "citext", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleName, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "references",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleName",
                        column: x => x.RoleName,
                        principalSchema: "references",
                        principalTable: "Roles",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AirlineHandlingContractFlightNumbers",
                schema: "flights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<int>(type: "integer", nullable: false),
                    FlightNumber = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineHandlingContractFlightNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirlineHandlingContractFlightNumbers_AirlineHandlingContrac~",
                        column: x => x.ContractId,
                        principalSchema: "references",
                        principalTable: "AirlineHandlingContracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArrivalFlightPassengers",
                schema: "bags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FlightId = table.Column<int>(type: "integer", nullable: false),
                    SecurityNumber = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    SequenceNumber = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    Seat = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    PassengerName = table.Column<string>(type: "character varying(55)", maxLength: 55, nullable: false),
                    Destination = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    IsTransfer = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArrivalFlightPassengers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArrivalFlightPassengers_ArrivalFlights_FlightId",
                        column: x => x.FlightId,
                        principalSchema: "flights",
                        principalTable: "ArrivalFlights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArrivalFlightReconciliations",
                schema: "bags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FlightId = table.Column<int>(type: "integer", nullable: false),
                    SnapshotAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsFinal = table.Column<bool>(type: "boolean", nullable: false),
                    ExpectedBagCount = table.Column<int>(type: "integer", nullable: false),
                    UnloadedBagCount = table.Column<int>(type: "integer", nullable: false),
                    RemainingOnAircraftBagCount = table.Column<int>(type: "integer", nullable: false),
                    ToBeltBagCount = table.Column<int>(type: "integer", nullable: false),
                    DeliveredBagCount = table.Column<int>(type: "integer", nullable: false),
                    TransferBagCount = table.Column<int>(type: "integer", nullable: false),
                    MissingBagCount = table.Column<int>(type: "integer", nullable: false),
                    UnknownBagCount = table.Column<int>(type: "integer", nullable: false),
                    RushBagCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArrivalFlightReconciliations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArrivalFlightReconciliations_ArrivalFlights_FlightId",
                        column: x => x.FlightId,
                        principalSchema: "flights",
                        principalTable: "ArrivalFlights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartureFlightPassengers",
                schema: "bags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PassengerStatus = table.Column<short>(type: "smallint", nullable: false),
                    FlightId = table.Column<int>(type: "integer", nullable: false),
                    SecurityNumber = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    SequenceNumber = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    Seat = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    PassengerName = table.Column<string>(type: "character varying(55)", maxLength: 55, nullable: false),
                    Destination = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    IsOnward = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartureFlightPassengers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartureFlightPassengers_DepartureFlights_FlightId",
                        column: x => x.FlightId,
                        principalSchema: "flights",
                        principalTable: "DepartureFlights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartureFlightReconciliations",
                schema: "bags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FlightId = table.Column<int>(type: "integer", nullable: false),
                    SnapshotAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsFinal = table.Column<bool>(type: "boolean", nullable: false),
                    ExpectedBagCount = table.Column<int>(type: "integer", nullable: false),
                    LoadedBagCount = table.Column<int>(type: "integer", nullable: false),
                    OffloadedCount = table.Column<int>(type: "integer", nullable: false),
                    ToBeOffloadedCount = table.Column<int>(type: "integer", nullable: false),
                    WaitingToLoadBagCount = table.Column<int>(type: "integer", nullable: false),
                    MissingBagCount = table.Column<int>(type: "integer", nullable: false),
                    ReconciledBagCount = table.Column<int>(type: "integer", nullable: false),
                    ForceLoadedBagCount = table.Column<int>(type: "integer", nullable: false),
                    OnwardBagCount = table.Column<int>(type: "integer", nullable: false),
                    TransferLoadedBagCount = table.Column<int>(type: "integer", nullable: false),
                    TransferMissingBagCount = table.Column<int>(type: "integer", nullable: false),
                    NotBoardedPassengerBagCount = table.Column<int>(type: "integer", nullable: false),
                    RushBagCount = table.Column<int>(type: "integer", nullable: false),
                    PriorityBagCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartureFlightReconciliations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartureFlightReconciliations_DepartureFlights_FlightId",
                        column: x => x.FlightId,
                        principalSchema: "flights",
                        principalTable: "DepartureFlights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "references",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "references",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "references",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleName = table.Column<string>(type: "citext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleName });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleName",
                        column: x => x.RoleName,
                        principalSchema: "references",
                        principalTable: "Roles",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "references",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArrivalBags",
                schema: "bags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CheckedWeight = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    Class = table.Column<short>(type: "smallint", nullable: false),
                    ContainerId = table.Column<int>(type: "integer", nullable: true),
                    RushReason = table.Column<string>(type: "text", nullable: true),
                    IsTransfer = table.Column<bool>(type: "boolean", nullable: false),
                    ArrivalBaggageStatus = table.Column<short>(type: "smallint", nullable: false),
                    FlightPassengerId = table.Column<int>(type: "integer", nullable: false),
                    TagNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SourceIndicator = table.Column<char>(type: "character(1)", maxLength: 1, nullable: false),
                    SourceAirportCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    HhtId = table.Column<int>(type: "integer", nullable: true),
                    UserName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArrivalBags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArrivalBags_ArrivalFlightPassengers_FlightPassengerId",
                        column: x => x.FlightPassengerId,
                        principalSchema: "bags",
                        principalTable: "ArrivalFlightPassengers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DepartureBags",
                schema: "bags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuthorityToLoad = table.Column<bool>(type: "boolean", nullable: false),
                    AckRequest = table.Column<char>(type: "character(1)", nullable: true),
                    AuthorityToTransport = table.Column<bool>(type: "boolean", nullable: false),
                    WeightIndicator = table.Column<char>(type: "character(1)", maxLength: 1, nullable: true),
                    CheckedWeight = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    OffloadReason = table.Column<int>(type: "integer", nullable: true),
                    Class = table.Column<int>(type: "integer", nullable: false),
                    ContainerId = table.Column<int>(type: "integer", nullable: true),
                    RushReason = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    DepartureBaggageStatus = table.Column<short>(type: "smallint", nullable: false),
                    IsPassengerBoarded = table.Column<bool>(type: "boolean", nullable: false),
                    IsOnward = table.Column<bool>(type: "boolean", nullable: false),
                    Destination = table.Column<string>(type: "text", nullable: false),
                    FlightPassengerId = table.Column<int>(type: "integer", nullable: false),
                    TagNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SourceIndicator = table.Column<char>(type: "character(1)", maxLength: 1, nullable: false),
                    SourceAirportCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    HhtId = table.Column<int>(type: "integer", nullable: true),
                    UserName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartureBags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartureBags_DepartureFlightPassengers_FlightPassengerId",
                        column: x => x.FlightPassengerId,
                        principalSchema: "bags",
                        principalTable: "DepartureFlightPassengers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArrivalBagEvents",
                schema: "bags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BagId = table.Column<long>(type: "bigint", nullable: false),
                    MessageId = table.Column<long>(type: "bigint", nullable: true),
                    DeviceId = table.Column<int>(type: "integer", nullable: true),
                    UserName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EventId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Description = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EventTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArrivalBagEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArrivalBagEvents_ArrivalBags_BagId",
                        column: x => x.BagId,
                        principalSchema: "bags",
                        principalTable: "ArrivalBags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartureBagEvents",
                schema: "bags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BagId = table.Column<long>(type: "bigint", nullable: false),
                    MessageId = table.Column<long>(type: "bigint", nullable: true),
                    DeviceId = table.Column<int>(type: "integer", nullable: true),
                    UserName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EventId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Description = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EventTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartureBagEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartureBagEvents_DepartureBags_BagId",
                        column: x => x.BagId,
                        principalSchema: "bags",
                        principalTable: "DepartureBags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirlineClassMap_AirlineCode_SourceClass",
                schema: "references",
                table: "AirlineClassMap",
                columns: new[] { "AirlineCode", "SourceClass" })
                .Annotation("Npgsql:IndexInclude", new[] { "TargetClass" });

            migrationBuilder.CreateIndex(
                name: "IX_AirlineClassMapLog_Action_Timestamp",
                schema: "audits",
                table: "AirlineClassMapLog",
                columns: new[] { "Action", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AirlineHandlingContractFlightNumberLog_Action_Timestamp",
                schema: "audits",
                table: "AirlineHandlingContractFlightNumberLog",
                columns: new[] { "Action", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AirlineHandlingContractFlightNumbers_ContractId_FlightNumber",
                schema: "flights",
                table: "AirlineHandlingContractFlightNumbers",
                columns: new[] { "ContractId", "FlightNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AirlineHandlingContractLog_Action_Timestamp",
                schema: "audits",
                table: "AirlineHandlingContractLog",
                columns: new[] { "Action", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AirlineHandlingContracts_AirlineCode_IsActive",
                schema: "references",
                table: "AirlineHandlingContracts",
                columns: new[] { "AirlineCode", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_AirlineHandlingContracts_HandlingCompanyCode",
                schema: "references",
                table: "AirlineHandlingContracts",
                column: "HandlingCompanyCode");

            migrationBuilder.CreateIndex(
                name: "IX_AirlineHandlingContracts_ValidFrom_ValidTo",
                schema: "references",
                table: "AirlineHandlingContracts",
                columns: new[] { "ValidFrom", "ValidTo" });

            migrationBuilder.CreateIndex(
                name: "IX_ArrivalBagEvents_BagId",
                schema: "bags",
                table: "ArrivalBagEvents",
                column: "BagId")
                .Annotation("Npgsql:IndexInclude", new[] { "EventId", "Description", "UserName", "EventTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ArrivalBags_FlightPassengerId",
                schema: "bags",
                table: "ArrivalBags",
                column: "FlightPassengerId")
                .Annotation("Npgsql:IndexInclude", new[] { "TagNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_ArrivalBags_TagNumber",
                schema: "bags",
                table: "ArrivalBags",
                column: "TagNumber")
                .Annotation("Npgsql:IndexInclude", new[] { "FlightPassengerId" });

            migrationBuilder.CreateIndex(
                name: "IX_ArrivalFlightLog_Action_Timestamp",
                schema: "audits",
                table: "ArrivalFlightLog",
                columns: new[] { "Action", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_ArrivalFlightPassengers_FlightId",
                schema: "bags",
                table: "ArrivalFlightPassengers",
                column: "FlightId")
                .Annotation("Npgsql:IndexInclude", new[] { "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_ArrivalFlightPassengers_FlightId_SecurityNumber",
                schema: "bags",
                table: "ArrivalFlightPassengers",
                columns: new[] { "FlightId", "SecurityNumber" },
                unique: true,
                filter: "\"SecurityNumber\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ArrivalFlightPassengers_FlightId_SequenceNumber",
                schema: "bags",
                table: "ArrivalFlightPassengers",
                columns: new[] { "FlightId", "SequenceNumber" },
                unique: true,
                filter: "\"SequenceNumber\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ArrivalFlightReconciliations_FlightId",
                schema: "bags",
                table: "ArrivalFlightReconciliations",
                column: "FlightId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArrivalFlights_AirlineCode_FlightNumber_FlightIataDate",
                schema: "flights",
                table: "ArrivalFlights",
                columns: new[] { "AirlineCode", "FlightNumber", "FlightIataDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ArrivalFlights_HandlingCompanyCode",
                schema: "flights",
                table: "ArrivalFlights",
                column: "HandlingCompanyCode");

            migrationBuilder.CreateIndex(
                name: "IX_ArrivalFlights_RemoteSystemId",
                schema: "flights",
                table: "ArrivalFlights",
                column: "RemoteSystemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyLog_Action_Timestamp",
                schema: "audits",
                table: "CompanyLog",
                columns: new[] { "Action", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_ContainerLog_Action_Timestamp",
                schema: "audits",
                table: "ContainerLog",
                columns: new[] { "Action", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_ContainerTypeClassLog_Action_Timestamp",
                schema: "audits",
                table: "ContainerTypeClassLog",
                columns: new[] { "Action", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_ContainerTypeLog_Action_Timestamp",
                schema: "audits",
                table: "ContainerTypeLog",
                columns: new[] { "Action", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_DepartureBagEvents_BagId",
                schema: "bags",
                table: "DepartureBagEvents",
                column: "BagId")
                .Annotation("Npgsql:IndexInclude", new[] { "EventId", "Description", "UserName", "EventTime" });

            migrationBuilder.CreateIndex(
                name: "IX_DepartureBags_FlightPassengerId_TagNumber",
                schema: "bags",
                table: "DepartureBags",
                columns: new[] { "FlightPassengerId", "TagNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_DepartureFlightContainers_FlightId",
                schema: "flights",
                table: "DepartureFlightContainers",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartureFlightLog_Action_Timestamp",
                schema: "audits",
                table: "DepartureFlightLog",
                columns: new[] { "Action", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_DepartureFlightPassengers_FlightId",
                schema: "bags",
                table: "DepartureFlightPassengers",
                column: "FlightId")
                .Annotation("Npgsql:IndexInclude", new[] { "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_DepartureFlightPassengers_FlightId_SecurityNumber",
                schema: "bags",
                table: "DepartureFlightPassengers",
                columns: new[] { "FlightId", "SecurityNumber" },
                unique: true,
                filter: "\"SecurityNumber\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DepartureFlightPassengers_FlightId_SequenceNumber",
                schema: "bags",
                table: "DepartureFlightPassengers",
                columns: new[] { "FlightId", "SequenceNumber" },
                unique: true,
                filter: "\"SequenceNumber\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DepartureFlightReconciliations_FlightId",
                schema: "bags",
                table: "DepartureFlightReconciliations",
                column: "FlightId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartureFlights_AirlineCode_FlightNumber_FlightIataDate",
                schema: "flights",
                table: "DepartureFlights",
                columns: new[] { "AirlineCode", "FlightNumber", "FlightIataDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DepartureFlights_HandlingCompanyCode",
                schema: "flights",
                table: "DepartureFlights",
                column: "HandlingCompanyCode");

            migrationBuilder.CreateIndex(
                name: "IX_DepartureFlights_RemoteSystemId",
                schema: "flights",
                table: "DepartureFlights",
                column: "RemoteSystemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HandheldTerminalLog_Action_Timestamp",
                schema: "audits",
                table: "HandheldTerminalLog",
                columns: new[] { "Action", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Hhts_AssignedCompanyCode",
                schema: "references",
                table: "Hhts",
                column: "AssignedCompanyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Hhts_DeviceId",
                schema: "references",
                table: "Hhts",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionLog_Action_Timestamp",
                schema: "audits",
                table: "PermissionLog",
                columns: new[] { "Action", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Group",
                schema: "references",
                table: "Permissions",
                column: "Group");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                schema: "references",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                schema: "references",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "references",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceStatusMapLog_Action_Timestamp",
                schema: "audits",
                table: "ResourceStatusMapLog",
                columns: new[] { "Action", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_RoleLog_Action_Timestamp",
                schema: "audits",
                table: "RoleLog",
                columns: new[] { "Action", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                schema: "references",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                schema: "references",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemConfigurationLog_Action_Timestamp",
                schema: "audits",
                table: "SystemConfigurationLog",
                columns: new[] { "Action", "Timestamp" });

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

            migrationBuilder.CreateIndex(
                name: "IX_UserLog_Action_Timestamp",
                schema: "audits",
                table: "UserLog",
                columns: new[] { "Action", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleName",
                schema: "references",
                table: "UserRoles",
                column: "RoleName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyCode",
                schema: "references",
                table: "Users",
                column: "CompanyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                schema: "references",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirlineClassMap",
                schema: "references");

            migrationBuilder.DropTable(
                name: "AirlineClassMapLog",
                schema: "audits");

            migrationBuilder.DropTable(
                name: "AirlineHandlingContractFlightNumberLog",
                schema: "audits");

            migrationBuilder.DropTable(
                name: "AirlineHandlingContractFlightNumbers",
                schema: "flights");

            migrationBuilder.DropTable(
                name: "AirlineHandlingContractLog",
                schema: "audits");

            migrationBuilder.DropTable(
                name: "ArrivalBagEvents",
                schema: "bags");

            migrationBuilder.DropTable(
                name: "ArrivalFlightLog",
                schema: "audits");

            migrationBuilder.DropTable(
                name: "ArrivalFlightReconciliations",
                schema: "bags");

            migrationBuilder.DropTable(
                name: "CompanyLog",
                schema: "audits");

            migrationBuilder.DropTable(
                name: "ContainerLog",
                schema: "audits");

            migrationBuilder.DropTable(
                name: "ContainerTypeClasses",
                schema: "references");

            migrationBuilder.DropTable(
                name: "ContainerTypeClassLog",
                schema: "audits");

            migrationBuilder.DropTable(
                name: "ContainerTypeLog",
                schema: "audits");

            migrationBuilder.DropTable(
                name: "ContainerTypes",
                schema: "references");

            migrationBuilder.DropTable(
                name: "DepartureBagEvents",
                schema: "bags");

            migrationBuilder.DropTable(
                name: "DepartureFlightContainers",
                schema: "flights");

            migrationBuilder.DropTable(
                name: "DepartureFlightLog",
                schema: "audits");

            migrationBuilder.DropTable(
                name: "DepartureFlightReconciliations",
                schema: "bags");

            migrationBuilder.DropTable(
                name: "ElementErrors",
                schema: "messages");

            migrationBuilder.DropTable(
                name: "HandheldTerminalLog",
                schema: "audits");

            migrationBuilder.DropTable(
                name: "Hhts",
                schema: "references");

            migrationBuilder.DropTable(
                name: "MessageErrors",
                schema: "messages");

            migrationBuilder.DropTable(
                name: "PermissionLog",
                schema: "audits");

            migrationBuilder.DropTable(
                name: "ReconciliationRecordSet");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "references");

            migrationBuilder.DropTable(
                name: "ResourceMaps",
                schema: "references");

            migrationBuilder.DropTable(
                name: "ResourceStatusMapLog",
                schema: "audits");

            migrationBuilder.DropTable(
                name: "RoleLog",
                schema: "audits");

            migrationBuilder.DropTable(
                name: "RolePermissions",
                schema: "references");

            migrationBuilder.DropTable(
                name: "SystemConfigurationLog",
                schema: "audits");

            migrationBuilder.DropTable(
                name: "SystemConfigurations",
                schema: "references");

            migrationBuilder.DropTable(
                name: "TextMessages",
                schema: "messages");

            migrationBuilder.DropTable(
                name: "UserLog",
                schema: "audits");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "references");

            migrationBuilder.DropTable(
                name: "AirlineHandlingContracts",
                schema: "references");

            migrationBuilder.DropTable(
                name: "ArrivalBags",
                schema: "bags");

            migrationBuilder.DropTable(
                name: "DepartureBags",
                schema: "bags");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "references");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "references");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "references");

            migrationBuilder.DropTable(
                name: "ArrivalFlightPassengers",
                schema: "bags");

            migrationBuilder.DropTable(
                name: "DepartureFlightPassengers",
                schema: "bags");

            migrationBuilder.DropTable(
                name: "ArrivalFlights",
                schema: "flights");

            migrationBuilder.DropTable(
                name: "DepartureFlights",
                schema: "flights");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "references");
        }
    }
}
