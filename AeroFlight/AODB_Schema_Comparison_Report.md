# AODB Schema vs AeroFlight.Domain — Comparison Report

**Database:** `AODB` on SQL Server (localhost:1433)  
**Date:** 2026-06-29

---

## Summary

| Category | Count |
|---|---|
| Entities with good match | 6 |
| Entities with missing/wrong fields | 11 |
| Entirely missing entities | 16 |
| Type/key mismatches | 8 |

---

## 1. FLIGHTS

### `OP_ARRIVALS` → `ArrivalFlight` — PARTIAL MATCH

#### ✅ Matched fields
| DB Column | Entity Property |
|---|---|
| OA_ID (numeric) | Id (long) |
| OA_FLIGHT_NUMBER | FlightNumber |
| OA_CALLSIGN | CallSign |
| OA_RAL_CODE | AirlineCode |
| OA_RFS_CODE | Status (FlightStatus enum) |
| OA_STA | Sta |
| OA_ETA | Eta |
| OA_ATA | Ata |
| OA_ONBLOCK | OnBlock |
| OA_TMO | Tmo |
| OA_RAP_CODE_ORIGIN | OriginAirportCode |
| OA_RAP_CODE_PREVIOUS | PreviousAirportCode |
| OA_RACT_CODE | AircraftTypeCode |
| OA_RAC_CODE | AircraftCode |
| OA_RCT_CODE | CountryType |
| OA_RSP_CODE | SpotCode |
| OA_GATES | Gates |
| OA_BAGGAGEBELTS | BaggageBelts |
| OA_CANCELLATION_TIME | CancellationDateTime |
| OA_CREATE/UPDATE | AuditBase |

#### ❌ Missing fields
| DB Column | Type | Notes |
|---|---|---|
| OA_GA_IND | char(1) | General aviation indicator |
| OA_OD_PAIR_ID | numeric | Pair departure flight ID |
| OA_OA_MAINFLIGHT | numeric | Main flight ID (for code shares) |
| OA_CODESHARE_NUMBER | numeric | Code share sequence FK |
| OA_PTA | datetime | Published time of arrival |
| OA_ETA_AIRSPACE_MONITOR | datetime | Airspace monitor ETA |
| OA_BTA | datetime | Block time arrival |
| OA_ISOLATED_IND | char(1) | Isolated parking indicator |
| OA_AC_CONFIGURATION | nvarchar(64) | Aircraft cabin configuration |
| OA_RAL_AC_AIRLINE | nvarchar(3) | Operating airline (vs marketing) |
| OA_AC_OWNER | varchar(64) | Aircraft owner |
| OA_RAP_CODE_DIVERT | nvarchar(4) | Divert airport code |
| OA_RRW_CODE | nvarchar(6) | Runway code |
| OA_RCON_CODE | nvarchar(5) | Concourse code |
| OA_RDI_CODE | char(1) | Direction indicator (I/O) |
| OA_RRM_CODE | nvarchar(5) | Remark code |
| OA_INTERNAL_COMMENT | nvarchar(2000) | Internal remark |
| OA_PUBLIC_COMMENT | nvarchar(2000) | Public/FIDS comment |
| OA_RDT_CODE | nvarchar(16) | Display text code |
| OA_RDBS_CODE | char(1) | Deboarding status code |
| OA_DEBOARDING_BEGIN | datetime | Deboarding start |
| OA_DEBOARDING_END | datetime | Deboarding end |
| OA_GATE_PLB_IND | char(1) | Passenger loading bridge used |
| OA_GATE_PCA_IND | char(1) | Pre-conditioned air used |
| OA_GATE_CU_IND | char(1) | Common use gate |
| OA_BAGGAGEBELT_BEGIN | datetime | Belt operation start |
| OA_BAGGAGEBELT_LASTBAG | datetime | Last bag time |
| OA_BAGGAGEBELT_END | datetime | Belt operation end |
| OA_BAGGAGEBELT_AREA | nvarchar(10) | Baggage claim area |
| OA_PAX_TOTAL | numeric | Total passenger count |
| OA_PAX_TRANSIT | numeric | Transit passengers |
| OA_PAX_ASSISTANCE_REQUIRED | numeric | Special assistance PAX |
| OA_PAX_1..6 | numeric | PAX by class (6 classes) |
| OA_CREWNUMBER | numeric | Total crew count |
| OA_CREW_COCKPIT | numeric | Cockpit crew count |
| OA_BAGGAGE_WEIGHT | numeric | Total baggage weight |
| OA_BAGGAGE_TOTAL | numeric | Total baggage count |
| OA_CARGO_WEIGHT | numeric | Cargo weight |
| OA_MAIL_WEIGHT | numeric | Mail weight |
| OA_TOTAL_WEIGHT | numeric | Total aircraft weight |
| OA_EQUIPMENT_WEIGHT | numeric | Equipment weight |
| OA_CABIN_LUGGAGE | numeric | Cabin luggage count |
| OA_RSP_PENDING_SPOT | nvarchar(5) | Pending/proposed spot |
| OA_RGT_PENDING_GATE | nvarchar(5) | Pending/proposed gate |
| OA_RBB_PENDING_BELT | nvarchar(5) | Pending/proposed belt |
| OA_CONFIRMED_BY | nvarchar(16) | Confirmation user |
| OA_CREDIT_STATUS | numeric | Credit/billing status |
| OA_QUALITIES | nvarchar(1000) | Quality flags JSON |
| OA_PREFERENTIAL_IND | char(1) | Preferential allocation |
| OA_SALES_TICKET | nvarchar(256) | Sales/billing reference |

#### ⚠️ Type/naming mismatches
| DB Column | DB Type | Our Property | Our Type | Issue |
|---|---|---|---|---|
| OA_RTRM_CODE | nvarchar(5) | TerminalId | int? | DB stores a **string code**, not int FK |
| OA_RAA_CODE | nvarchar(10) | HandlingId | int? | DB stores a **string code** (business partner), not int FK |

---

### `OP_DEPARTURES` → `DepartureFlight` — PARTIAL MATCH

Same structure as ArrivalFlight. Additional departure-specific missing fields:

| DB Column | Type | Notes |
|---|---|---|
| OD_PTD | datetime | Published departure time |
| OD_BTD | datetime | Block departure time |
| OD_BOARDING_GOTO_GATE | datetime | Go to gate time |
| OD_BOARDING_BEGIN | datetime | Boarding start |
| OD_BOARDING_SECOND | datetime | Second boarding call |
| OD_BOARDING_END | datetime | Boarding end |
| OD_RBOS_CODE | char(1) | Boarding status |
| OD_RCIS_CODE | char(1) | Check-in status |
| OD_COUNTER_BEGIN | datetime | Counter open time |
| OD_COUNTER_END | datetime | Counter close time |
| OD_BELTS | nvarchar(16) | Departure belt display string |
| OD_ISOLATED_IND | char(1) | Isolated parking |
| OD_AC_CONFIGURATION | nvarchar(64) | Cabin config |
| + all PAX/weight fields as above | | |

---

## 2. RESOURCE ALLOCATIONS

### `OP_ARRIVAL_BELTS` → `ArrivalBelt` — PARTIAL MATCH

#### ❌ Missing fields
| DB Column | Notes |
|---|---|
| OAB_RBB_LAST | Last belt code (history tracking) |
| OAB_RBES_CODE | Belt status code (FK to REF_BELT_STATUSES) — DB uses string |
| OAB_RDT_CODE | Display text code |
| OAB_RDO_ID | Display object ID |
| OAB_COMMENT | Operational comment |
| OAB_NOTE | Additional note |
| OAB_RAA_CODE | Handling agent code |

> ⚠️ `ResourceStatus` enum doesn't match: DB uses a string code `RBES_CODE` → `char(1)` FK.  
> Also: DB has `OAB_LAST_BAG` but NOT `FirstBaggage`. `FirstBaggage` on `ArrivalBelt` doesn't exist in DB.

---

### `OP_ARRIVAL_GATES` → `ArrivalGate` — PARTIAL MATCH

Missing: `LastCode`, `DisplayTextCode`, `DisplayObjectId`, `Comment`, `Note`, `HandlingAgentCode`, `Status` (string FK to gate status)

---

### `OP_DEPARTURE_GATES` → `DepartureGate` — PARTIAL MATCH

Missing everything above plus:
| DB Column | Notes |
|---|---|
| ODG_BOARDING_PLAN | Boarding time plan |
| ODG_BOARDING_ACTUAL | Boarding time actual |

---

### `OP_DEPARTURE_COUNTERS` → `DepartureCounter` — PARTIAL MATCH

Missing: `LastCode`, `DisplayTextCode`(×3), `DisplayObjectId`, `Comment`, `Note`, `CheckinClassCode`, `CiClassEffect`, `CommentEffect`, `DisplayTextEffect`, `HandlingAgentCode`

---

### `OP_ARRIVAL_DELAYS` / `OP_DEPARTURE_DELAYS` → `ArrivalDelay` / `DepartureDelay` — TYPE MISMATCH

| DB Column | DB Type | Our Property | Our Type | Issue |
|---|---|---|---|---|
| OAD_RDLR_CODE | nvarchar(3) | DelayId | int | DB uses a **string code** (e.g. "93") FK to REF_DELAY_REASONS, not an int |
| OAD_DELAY | numeric | — | — | Delay duration in minutes — MISSING |
| OAD_COMMENT | nvarchar(256) | — | — | Delay comment — MISSING |

---

### `OP_ARRIVAL_ROUTINGS` / `OP_DEPARTURE_ROUTINGS` → `ArrivalRouting` / `DepartureRouting` — WRONG STRUCTURE

The Routing entity is completely redesigned in the real DB. It is NOT a display-route string:

| DB Column | Type | Notes |
|---|---|---|
| OAR_RAP_CODE | nvarchar(4) | **Airport code** for this leg stop |
| OAR_LEG_NUMBER | numeric | Leg sequence number (not OrderId) |
| OAR_STA_STATION | datetime | STA at this station |
| OAR_STD_STATION | datetime | STD at this station |
| OAR_ONBLOCK | datetime | On-block at this station |
| OAR_OFFBLOCK | datetime | Off-block at this station |
| OAR_RDI_CODE | char(1) | Direction (I/O) |

> **Our `Routing` entity must be rebuilt**: `OrderId`→`LegNumber`, `Route`→`AirportCode`, drop `DisplayCode`, add `Sta`, `Std`, `OnBlock`, `OffBlock`, `DirectionCode`.

---

### `OP_ARRIVAL_SERVICES` / `OP_DEPARTURE_SERVICES` → `ArrivalService` / `DepartureService` — MOSTLY OK

Missing: `HandlingAgentCode` (OAS_RAA_CODE), `Comment`.  
⚠️ Our `PayerCompanyId` (int) has **no equivalent in the DB** — remove it.

---

### `OP_ARRIVAL_AGENTS` / `OP_DEPARTURE_AGENTS` → `ArrivalAirportService` / `DepartureAirportService` — WRONG CONCEPT

The DB `OP_ARRIVAL_AGENTS` is a **handling agent assignment** (linking a handling company to a flight), not an "airport service":

| DB Column | Notes |
|---|---|
| OAA_RHT_CODE | Handling type code (FK to REF_HANDLING_TYPES) |
| OAA_RBP_CODE | Business partner code (FK to REF_BUSINESS_PARTNERS) |
| OAA_COMMENT | Comment |

> **Rename** `AirportService` → `HandlingAgent`. Properties: `HandlingTypeCode`, `BusinessPartnerCode`, `Comment`.

---

### `OP_SPOTS` → `Spot` — PARTIAL MATCH

Missing fields:

| DB Column | Notes |
|---|---|
| OS_RSP_LAST | Previous/last spot code |
| OS_ORDER | Sequence order |
| OS_TOW_CODE | Tow type code |
| OS_TOW_IND | Tow indicator |
| OS_RAA_CODE | Handling agent code |
| OS_RFS_CODE | Flight status at spot allocation |
| OS_PREFERENTIAL_IND | Preferential allocation |
| OS_SALES_TICKET | Billing reference |

> Also: `OS_OA_ID` and `OS_OD_ID` are **nullable** — a spot can belong to only arrival OR only departure.

---

## 3. ENTIRELY MISSING OPERATIONAL ENTITIES

| Table | Entity Needed | Notes |
|---|---|---|
| OP_TRANSFER_PASSENGERS | `TransferPassenger` | Inter-flight passenger transfer records |
| OP_DEPARTURE_MEAL_CODES | `DepartureMealCode` | Meal codes per cabin class |
| OP_COMMON_CHECK_INS | `CommonCheckIn` | Shared check-in resources across flights |
| OP_GENERIC_ALLOCATIONS | `GenericAllocation` | Generic resource type allocation |

---

## 4. METADATA / REFERENCE ENTITIES

### `REF_TERMINALS` → `Terminal` — GOOD MATCH ✅

Minor: DB has `Description` only, no separate `Name` field. Our `Name` property is extra (not wrong).

---

### `REF_GATES` → `Gate` — PARTIAL MATCH

Missing fields:

| DB Column | Notes |
|---|---|
| RGT_RCON_CODE | **Concourse** code (not terminal directly!) |
| RGT_RGAT_CODE | Gate type code |
| RGT_CAPACITY | Passenger capacity |
| RGT_RAL_CODE | Dedicated airline code |
| RGT_LEASED_IND | Is leased |
| RGT_RIO_CODE | Inbound/outbound type |
| RGT_PAXLOADINGBRIDGE_IND | Has jetway |
| RGT_PCA_IND | Pre-conditioned air available |
| RGT_COMMONUSE_IND | Common use gate |
| RGT_SR_CATEGORY | Service category |

> ⚠️ Gates link to **Concourse**, not directly to Terminal. `TerminalId` must become `ConcourseCode`.

---

### `REF_BAGGAGE_BELTS` → `Belt` — PARTIAL MATCH

Missing: `Area`, `Capacity`, `BaggageCapacity`, `AirlineCode`, `IsLeased`, `BcisHostname`.  
⚠️ Links to **Concourse** (RCON_CODE), not terminal.

---

### `REF_COUNTERS` → `Counter` — PARTIAL MATCH

Missing: `CounterGroup`, `Capacity`, `AirlineCode`, `IsLeased`, `IsCommonUse`, `HandlingAgentCode`, `ServiceCategory`.  
⚠️ Links to **Concourse** (RCON_CODE), not terminal.

---

### `REF_AIRLINES` → `Airline` — PARTIAL MATCH

#### ⚠️ Key mismatch
DB uses a **string code** (RAL_CODE nvarchar(3)) as the PK, not an int. Our `AuditBase<int>` is wrong — should be `AuditBase<string>`.

Missing fields: `IataCode2`, `IcaoCode2`, `Callsign`, `NameShort`, `NameMedium`, `NameLong`, `ParentAirlineCode`, `Alliance`, `ServiceTypeCode`, `IsHomeCarrier`, `SfsIgnoreInd`, `SitaAddress`, `ServiceCategory`.

---

### `REF_AIRCRAFT_TYPES` → `AircraftType` — PARTIAL MATCH

#### ⚠️ Key mismatch
DB uses a **string code** (RACT_CODE nvarchar(5)) as the PK, not an int.

Missing: `IataCode2`, `IcaoCode`, `SizeCategory`, `PropulsionType`, `NoiseCategory`, `MTOW`, `LandingWeight`, `BridgeRequired`.

---

## 5. ENTIRELY MISSING METADATA ENTITIES

| Table | Entity Needed | Key Notes |
|---|---|---|
| REF_AIRPORTS | `Airport` | IATA/ICAO codes, timezone, country — critical for routing |
| REF_AIRCRAFT | `Aircraft` | Specific tail numbers, owner, seat capacity |
| REF_CONCOURSES | `Concourse` | Between Terminal and Gate/Belt/Counter |
| REF_SPOTS | `Stand` | Parking stands (ref data), GPS coords, spot type |
| REF_BUSINESS_PARTNERS | `BusinessPartner` | Handling companies / agents |
| REF_HANDLING_TYPES | `HandlingType` | Types of ground handling services |
| REF_DELAY_REASONS | `DelayReason` | IATA delay codes with alphabetic + numeric codes |
| REF_SERVICES | `ServiceDefinition` | Service catalogue (catering, fuel, cleaning…) |
| REF_RUNWAYS | `Runway` | Runway length, width, code |
| REF_REMARKS | `Remark` | Predefined remarks/annotations for flights |
| REF_FLIGHT_STATUSES | `FlightStatusDefinition` | Rich status config (attachable, updatable flags) |
| REF_BELT_STATUSES | `BeltStatusDefinition` | Belt status codes with order |
| REF_GATE_TYPES | `GateType` | Gate type lookup |
| REF_COUNTRY_TYPES | `CountryType` (entity) | Domestic/Schengen/International codes |
| REF_CHECKIN_CLASSES | `CheckinClass` | Cabin class codes (F, C, Y…) |

---

## 6. SCHEDULE ENTITIES

### `SFS_SEASONS` → `ScheduleSeason` — GOOD MATCH ✅

DB has no `Description` field. Our extra `Description` property is harmless.

---

### `SFS_AIRLINE_VERSIONS` → `SfAirlineVersion` — PARTIAL MATCH

| DB Column | Notes |
|---|---|
| SAV_VERSIONNUMBER | numeric — our `AirlineVersion` is string, should be **int** |
| SAV_ACTIVE_IND | char(1) — **IsActive** flag MISSING |
| SAV_TITLE | our `Description` ✅ |

---

### `SFS_ARRIVALS` → `SfArrivalFlight` — PARTIAL MATCH

Many fields missing. Most important:

| DB Column | Notes |
|---|---|
| SA_PTA | Published STA |
| SA_PERIOD_BEGIN / SA_PERIOD_END | Season validity period per flight |
| SA_FREQUENCY | Flight frequency |
| SA_OVER_MIDNIGHT_IND | Crosses midnight flag |
| SA_RAP_CODE_ORIGIN | Origin airport — MISSING |
| SA_RAP_CODE_PREVIOUS | Previous airport — MISSING |
| SA_RTRM_CODE | Terminal code — MISSING |
| SA_RSP_CODE | Default spot — MISSING |
| SA_RGT_CODE | Default gate — MISSING |
| SA_TRANSIT_IND | Transit indicator — MISSING |
| SA_COMMENT | Comment — MISSING |
| SA_RSFS_CODE | Schedule flight status — MISSING |

---

## 7. IDENTITY / SYSTEM

### `SYS_USERS` → `User` — TYPE MISMATCH

| DB Column | DB Type | Our Property | Our Type | Issue |
|---|---|---|---|---|
| SUS_CODE | nvarchar(16) | — | Guid (PK) | **DB uses a string username as PK**, not Guid |

Missing: `PasswordExpirationDate`, `MultipleLoginsAllowed`, `LastPasswords`, `ExpirationDate`.  
Our `FirstName`/`LastName` maps to DB's `SUS_REAL_NAME` (single field).

---

### `SYS_ROLES` → `Role` — GOOD MATCH ✅

---

### `SYS_PARAMETERS` → `AeroConfiguration` — PARTIAL MATCH

Missing: `Category`, `Unit`, `Format`, `IsSession` (SYP_SESSION_IND).  
⚠️ DB PK is a **string code** (SYP_CODE nvarchar 64), not int — key mismatch.

> There is also **`SYS_PROPERTIES`** (simple code/value store) — completely missing.

---

## 8. CONSOLIDATED ISSUES LIST

### 🔴 Critical — Wrong structure
1. **Routing** — completely wrong; must represent per-leg airport stops with STA/STD/OnBlock/OffBlock
2. **AirportService** — wrong concept; must be renamed to `HandlingAgent` with `HandlingTypeCode` + `BusinessPartnerCode`
3. **Delay.DelayId** — must be `string DelayCode` (nvarchar 3), not `int`

### 🟠 Key/type mismatches
4. **Flight.TerminalId** (`int?`) → `TerminalCode` (`string?`)
5. **Flight.HandlingId** (`int?`) → `HandlingAgentCode` (`string?`)
6. **Airline.Id** (`int`) → string-keyed (`AuditBase<string>`, PK = IATA/internal code)
7. **AircraftType.Id** (`int`) → string-keyed (PK = RACT_CODE nvarchar(5))
8. **User.Id** (`Guid`) → string-keyed (PK = nvarchar(16) username)
9. **AeroConfiguration.Id** (`int`) → string-keyed (PK = nvarchar(64) code)
10. **SfAirlineVersion.AirlineVersion** (`string`) → `VersionNumber` (`int`)
11. **Gate/Belt/Counter.TerminalId** — must link via **Concourse**, not directly to Terminal

### 🟡 Missing fields in existing entities
12. Passenger/weight data block (~15 fields) on both `ArrivalFlight` and `DepartureFlight`
13. Boarding/deboarding time fields on flights
14. Pending allocation fields (PendingSpot, PendingGate, PendingBelt) on flights
15. `PublicComment`, `InternalComment` on flights
16. `LastCode` (previous resource code) on all resource assignment entities
17. `HandlingAgentCode` on resource assignment entities
18. `Comment` / `Note` on resource assignment entities
19. `IsActive` on `SfAirlineVersion`
20. Delay duration minutes + comment on `Delay` entities
21. `FirstBaggage` on `ArrivalBelt` — does not exist in DB (remove it)

### 🟢 Entirely missing entities (to add)
22. `Airport` (REF_AIRPORTS)
23. `Aircraft` (REF_AIRCRAFT)
24. `Concourse` (REF_CONCOURSES)
25. `Stand` (REF_SPOTS reference data)
26. `BusinessPartner` (REF_BUSINESS_PARTNERS)
27. `HandlingType` (REF_HANDLING_TYPES)
28. `DelayReason` (REF_DELAY_REASONS)
29. `ServiceDefinition` (REF_SERVICES)
30. `Runway` (REF_RUNWAYS)
31. `Remark` (REF_REMARKS)
32. `FlightStatusDefinition` (REF_FLIGHT_STATUSES)
33. `GateType` (REF_GATE_TYPES)
34. `CountryTypeDefinition` (REF_COUNTRY_TYPES)
35. `CheckinClass` (REF_CHECKIN_CLASSES)
36. `DepartureBelt` metadata entity (REF_DEPARTURE_BELTS — separate from arrival belt!)
37. `TransferPassenger` (OP_TRANSFER_PASSENGERS)
38. `DepartureMealCode` (OP_DEPARTURE_MEAL_CODES)
39. `CommonCheckIn` (OP_COMMON_CHECK_INS)
40. `GenericAllocation` (OP_GENERIC_ALLOCATIONS)
41. `SysProperty` (SYS_PROPERTIES — simple key/value store)
