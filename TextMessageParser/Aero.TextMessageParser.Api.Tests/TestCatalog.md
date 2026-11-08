# Aero.TextMessageParser.Api.Tests - Test Catalog

Run everything: `dotnet test`. One bug: `dotnet test --filter "Bug=3"`. Baseline only: `--filter "Kind=Baseline"`.
Source guards (static reads of production source): `--filter "Kind=SourceGuard"`. Low priority: `--filter "Priority=Low"`.

Red = the bug is confirmed (the test asserts the CORRECT behavior). Every failure message carries the explanation of the bug.
There are no code comments in the tests; the explanation lives in the DisplayName, the `Why` text in each assertion message, and this file.
Environment note: results below were observed on Windows (`Environment.NewLine` = CRLF). Bug 13 behaves differently on Linux.

Shared samples (Infrastructure/SampleMessages.cs), joined with the native line ending:

```
BSM
.V/7LFRA
.F/LH400/12NOV/JFK/Y
.N/0220123456001
.P/DOE/JOHN
ENDBSM
```

Test infrastructure notes:
- `TestParserDbContext` (test project) subclasses the production DbContext and ignores the shared entities, because the production model cannot be built at all (see Bug 15). Without it no message test could run.
- `ApiFactory` boots the real `Program` with WebApplicationFactory, swaps Npgsql for InMemory and can remove the hosted service.
- Only production change: `<InternalsVisibleTo Include="Aero.TextMessageParser.Api.Tests" />` in the Api csproj. `public partial class Program` was NOT needed.

## Scope change: removed Scanner-related tests
The service will no longer talk to the Scanner API, so these were dropped and are not in the project:
- Bug 2 (HttpClient "scannerapi" `new Uri("")`, unused `RemoteServer:BaseUrl`): all tests removed.
- Bug 10 URI part (BaggageMessageDispatcher posting BSM/CHG/DEL to the same URI, fake HttpMessageHandler): removed. Only the JSON serialization of Onwards remains.
- Bug 1: no dispatcher/HttpClient assertions. Production has meanwhile switched to `IMessageProcessor` + `InstallProcessors()` and Program.cs now calls `InstallElements/InstallMessages/InstallProcessors`, so Bug 1 tests are green (see below).
- Bug 8: the reflective `DispatchMessageAsync` test was dropped because the dispatcher abstraction no longer exists.

## Bug 1 - Startup registration (fixed in current source, tests green)
| Test | Input | Expected | Actual |
|---|---|---|---|
| Program_host_registers_element_map_and_message_handlers | Real Program via WebApplicationFactory, hosted service removed | `IReadOnlyDictionary<string, Element>` and `Bsm` resolvable | Resolvable (green). Was missing before Program.cs began calling Install*. |
| Processing_service_dependencies_resolve | Real host container | `MessageProcessingService` constructible; keyed `IMessageProcessor` for BSM, BSMCHG, BSMDEL, BUM, BCMFOM, BCMFCM, BCMBAM, BCMDBM | Green |
| Host_starts_with_processing_service | Host with hosted service enabled | Host starts | SKIPPED: the service runs `ExecuteUpdateAsync` and PostgreSQL-only raw SQL, which InMemory cannot execute; needs PostgreSQL |
| Install_extensions_resolve_bsm (Baseline) | `InstallElements().InstallMessages()` | Bsm resolves, 24 elements | Green |

## Bug 3 - Stateful shared element instances (fixed)
| Test | Input | Expected | Actual | Why it matters |
|---|---|---|---|---|
| Three_n_lines_give_three_tags | BSM with `.N/0220123456001`, `.N/0220123457001`, `.N/0220123458001` | Tags 0220123456, 0220123457, 0220123458 | `[0220123458, 0220123458, 0220123458]` | One ElementN instance is reused; the DTO is built after all parsing, so every result shows the last line. Multi-bag passengers get the wrong tags. |
| Two_o_lines_give_two_distinct_onwards | `.O/LH401/13NOV/MUC/Y`, `.O/LH402/14NOV/BER/Y` | Destinations MUC, BER | `[BER, BER]` | Same shared-instance problem for ElementO. |
| Element_p_state_does_not_leak_between_messages | A: `.P/2DOE/JOHN/MIDDLE1/MIDDLE2`, then B: `.P/ROE` | B state: no additional names, no count, no given name | AdditionalGivenNames still has MIDDLE1, MIDDLE2 (list only appended) | Data of one passenger leaks into the next message. |
| Passenger_name_does_not_leak_into_dto | A: `.P/DOE/JOHN`, B: `.P/ROE` | B name without JOHN | `JOHN ROE` | Leak visible in the DTO. |
| Element_s_seat_does_not_leak (probe) | A: `.S/Y/12A/B/001/SEC123/Y/N/A`, B: `.S/Y` | B seat is null | null (green) | NOT a bug: ElementS assigns every property on each Parse. |
| Element_w_state_does_not_leak | A: `.W/K/001/20/CM/55/40/20/XYZ`, B: `.W/K/001/20` | Unit and LengthOfBag empty | Unit `CM` leaked | Unit/dimensions are only assigned inside one branch. |
| Concurrent_processing_does_not_mix_results | 32 threads x 20 rounds, unique serial and name each, one shared Bsm, Barrier-synchronised | Each DTO matches its own message | ~250-320 of 640 mixed (e.g. msg 28 got tag of msg 27) | Real concurrent processing corrupts data. (Count varies run to run.) |

## Bug 4 - Validators skip non-Mandatory elements (fixed)
| Test | Input | Expected | Actual |
|---|---|---|---|
| Malformed_element_is_rejected_when_mandatory (control, Baseline) | `.F/ .I/ .O/ .P/ .S/ .U/ .W/` as Mandatory | Rejected | Rejected (green); proves the samples are malformed |
| Malformed_element_is_rejected_regardless_of_requirement | The same 7 lines with Conditional and with Optional (14 cases) | Rejected | Green after the fix: the `Requirement != Mandatory` guard was removed from all validators, including ElementSValidator |
| Malformed_conditional_f_gives_error_not_exception | BSM with `.F/`, `.F/LH400`, `.F/LH400/12NOV`, `.F/XX/YY/ZZ/Q` | `ProcessMessage` returns null and an error row is recorded | Green after the fix |

## Bug 5 - ElementSValidator (fixed, follows IATA Passenger Services Conference Resolutions Manual 30th ed., section 2.3.22)
The field layout of `.S/` is: 1 Authority to Load (Y/N, the only mandatory field), 2 Seat number `f(f)a`, 3 Passenger status B/C/N/S, 4 Sequence number `fff`, 5 Security number `fff`, 6 Passenger profile status Y/N, 7 Authority to Transport Y/N, 8 Baggage tag status I/A. All fields after the first may be empty or omitted. The element is 6 to 26 characters long, where the count includes the 2 character end delimiter (this is why `ElementDetail.Length` is `2 + Text.Length`). This layout matches `ElementS.Parse`; the old validator checked other positions.
| Test | Input | Expected | Actual |
|---|---|---|---|
| ElementS_valid_lines_pass | `.S/Y`, `.S/N/4K`, `.S/Y//S/291` (specification example), full 8 field lines with I and A | Valid | Green |
| ElementS_invalid_values_fail | One invalid value per field, e.g. `.S/B` (S_04), `.S/Y/123` (S_05), `.S/Y/23A/Q` (S_01), `.S/Y/23A/C/01` (S_06), `...016/67` (S_07), `...678/Q` (S_02), `...Y/Q` (S_08), `...Y/Y/Z` (S_03) | Rejected with the field's own error code | Green |
| ElementS_length_is_enforced | `.S/`, a 28 character line | Rejected | Green |

## Bug 6 - Bsm/BsmChg mapping (fixed)
| Test | Input | Expected | Actual |
|---|---|---|---|
| Bsm_passenger_status_mapping | `.S/Y/12A/B/001/123/Y/N/A` (status B, profile Y) | DTO.PassengerStatus = `B` | Green after the fix (the DTO now gets the S passenger status) |
| BsmChg_passenger_status_mapping | Same, in BSM CHG | `B` | Green |
| Dto_exposes_weight (2 DTO types) | `.W/K/001/20` | A weight property exists on the DTO | Green: both DTOs have `BaggageWeight` (`BaggageWeightDto`) |
| Weight_values_reach_the_dto | `.W/K/001/20` in BSM and BSM CHG | Indicator `K`, `001` bags, checked weight `20` | Green |
| Absent_weight_is_null_and_profile_status_is_kept | BSM without `.W/`, S with profile Y | BaggageWeight null, PassengerStatus `B`, PassengerProfileStatus `Y` | Green (the DTOs gained an optional `PassengerProfileStatus`) |
| Empty_optional_s_fields_do_not_crash_the_parser | `.S/Y//S/291` | Parses, SeatNumber null, status `S`, sequence `291` | Green (ElementS.Parse no longer indexes into empty fields) |

## Bug 7 - Unknown element lines (fixed)
| Test | Input | Expected | Actual |
|---|---|---|---|
| Unknown_element_gives_validation_error | Extra line `.Z/XYZ`, `THIS IS FREE TEXT`, `AB`, `.U/CONT1/1/M/Y/FRA/N` (U is not in BSM) | null result and error code `UNKNOWN_ELEMENT_FOUND` | InvalidOperationException "Sequence contains no matching element" (4 red); the UNKNOWN_ELEMENT_FOUND branch is unreachable |

## Bug 8 - Processing failure handling
| Test | Result |
|---|---|
| Failed_message_is_persisted_as_terminal | SKIPPED: needs PostgreSQL (raw claim SQL, private method). Skip reason holds the reproduction: BUM message, observe Status stays PROCESSING, Completed false. |
| Catch_block_persists_failure (SourceGuard) | Green after the fix: the catch block calls `RecordFailureAsync`, which opens a fresh scope and writes Status ERROR, ProcessDateTime and a `MSG_HND_02` error row. |
| Stale_recovery_runs_inside_the_loop (SourceGuard) | Green after the fix: `RecoverStaleRecordsAsync` runs every minute inside the loop, not only at startup. |
| Bum_handler_throws_not_implemented | Green, documents that `Bum.Handle` throws NotImplementedException (what reaches the catch block). |

## Bug 9 - Raw SQL alias
| Test | Result |
|---|---|
| Raw_sql_returns_value_column | SKIPPED: needs PostgreSQL. Reason documents the expected EF error (column Value missing). |
| Claim_sql_aliases_id_as_value (SourceGuard) | RED: SQL ends with `RETURNING "Id"` without `AS "Value"`. |

## Bug 10 - JSON serialization of Onwards (fixed)
| Test | Input | Expected | Actual |
|---|---|---|---|
| Request_onwards_are_serialized (default and web options) | Two `OnwardFlightDto` records (LH401 13NOV MUC Y, EW9021 14NOV BER C) in TextMessageDepartureBagRequest | JSON contains airline, number, date, destination of each | Green after the fix. Before: onwards were ValueTuples, whose members are fields that System.Text.Json ignores, so each serialized as `{}` |
| Chg_onwards_are_serialized (default and web) | Same in TextMessageDepartureBagChgDto | Same | Green after the fix |
| Scalars_are_serialized (Baseline) | Scalars and bag tags | Present | Green |

## Bug 11 - BCM subtype
| Test | Input | Expected | Actual |
|---|---|---|---|
| Bcm_subtype FOM | `BCM / FOM / .V/7LFRA / .F/LH400/12NOV/JFK / ENDBCM` | `Fom` | `Fom` (green) |
| Bcm_subtype FCM | same with FCM | `Fcm` | `Fcm` (green) |
| Bcm_subtype DBM | same with DBM | `Dbm` | `Fcm` (RED, hard-coded in BcmDbm) |

## Bug 12 - Envelope handling (fixed)
| Test | Input | Expected | Actual |
|---|---|---|---|
| GetMessageIdentifier_degenerate_input_does_not_throw_index_exceptions | `null`, `""`, `"BSM"`, 9 spaces, `"\r\n\r\n\r\n"` | No index exception | ArgumentOutOfRangeException for all 5 inputs (5 red) |
| Degenerate_text_is_rejected_in_a_controlled_way | `"BSM"` | `TryGetMessageIdentifier` returns false, `GetMessageIdentifier` throws FormatException | Green after the fix (replaces the old test that documented the ArgumentOutOfRangeException) |
| Bsm_with_mismatching_end_line_is_rejected | BSM ending with `ENDBUM` | null + error row | Accepted |
| Bsm_without_end_line_is_rejected | BSM without any END line | null + error row | Accepted; `.P/DOE/JOHN` silently becomes the footer |
| Endpoint_returns_bad_request_for_degenerate_bodies | POST `{"message":null}`, `{}`, 9 spaces, `BSM-------` | 400 | 500 (exception middleware) |
| Endpoint_rejects_bad_footer | POST BSM with ENDBUM / without END | 400 | 200 (stored) |
| Endpoint_accepts_valid_bsm (Baseline) | Sample BSM | 200 and one row stored | Green |
| GetMessageIdentifier_reads_header_and_footer (Baseline) | BSM, BSM CHG, BSM DEL | Header, secondary identifier, footer | Green |

## Bug 13 - Newline handling (fixed)
| Test | Input | Expected | Actual (Windows) |
|---|---|---|---|
| GetMessageIdentifier_is_independent_of_line_ending LF | Sample BSM with `\n` | Header BSM, footer ENDBSM | ArgumentOutOfRangeException (`Span.Split(char[])` splits on the whole `\r\n` sequence, so LF-only text is one line) |
| same, CRLF | with `\r\n` | same | Green |
| ProcessMessage_is_independent_of_line_ending LF / CRLF | Sample BSM | Same DTO | LF red, CRLF green |
| NewLineSeparator_is_cached (Priority=Low) | Two reads of `Consts.NEW_LINE_SEPERATOR` | Same array instance | New array each time (red) |

On Linux the separator is `['\n']`, so CRLF text would keep a trailing `\r` on every line; that direction cannot be exercised on this machine.

## Bug 14 - Parsed result storage (RED)
Model_contains_parsed_result_storage: expects at least one non-owned entity from the Api assembly besides TextMessage, TextMessageError, ElementError. Actual: none. The schema decision belongs to the owner.

## Bug 15 - Additional finding: production DbContext model cannot be built (fixed)
Production_model_builds: `new TextMessageParserDbContext(options).Model` throws "The entity type 'Permission' requires a primary key". `DataStoreContext` has DbSets for User, Role, Permission and so on but applies configurations only from the Api assembly. Any real DbContext use (endpoint save, processing service) fails. Tests use `TestParserDbContext` to work around it.

## TextMessage entity behavior (clean architecture step)
`TextMessage` now owns its state transitions: `MarkProcessed` and `MarkFailed` complete a claimed (PROCESSING) message and throw `InvalidOperationException` otherwise. Setters are private; status values live in `TextMessageStatus`. The claim itself stays a set-based SQL statement (`FOR UPDATE SKIP LOCKED`), so there is deliberately no `MarkProcessing` method.
| Test | Input | Expected | Actual |
|---|---|---|---|
| New_message_state (Baseline) | `TextMessage.Create` | NOT_PROCESSED, not completed, RecordDateTime set | Green |
| Mark_processed_completes_a_claimed_message | Claimed message, `MarkProcessed(at)` | PROCESSED, Completed, ProcessDateTime = at | Green |
| Mark_failed_completes_a_claimed_message | Claimed message, `MarkFailed(at)` | ERROR, Completed, ProcessDateTime = at | Green |
| Completing_requires_a_claimed_message | Unclaimed message, and an already completed one | InvalidOperationException | Green |

## Parser without a database, shared mapping (clean architecture step)
`MessageBase<TResult>.Parse(TextMessage)` returns a `ParseResult<TResult>` (a value or a list of `ParseError`s, each flagged as message level or element level) and no longer knows the DbContext. `MessageParserExtensions.ProcessMessage(parser, message, db)` in the Services namespace parses and writes the errors to `MessageErrors` / `ElementErrors`; `MessageProcessor` uses it. BSM, BSM CHG and BSM DEL share `BsmFields` (reads the elements once) and `BsmMapper` (builds the DTO). BCM FOM, FCM and DBM share one `Handle` in `BcmBase`; the sub type comes from the identifier.
| Test | Input | Expected | Actual |
|---|---|---|---|
| Valid_message_succeeds_without_a_database (Baseline) | Single tag BSM | Success, no errors, one bag tag | Green |
| Message_level_problem_is_a_message_error | BSM ending with ENDBUM | One message level error `MSG_BSM_END_INVALID`, no value | Green |
| Element_level_problem_is_an_element_error | BSM with `.F/LH400` | One element level error, no value | Green |
| Bsm_and_chg_share_the_same_mapping | Same elements (two tags, onward, S, W, P) as BSM and BSM CHG | Identical JSON for both DTOs | Green |
| Delete_uses_the_shared_reader | BSM DEL with two tags | Both tags, airline, destination | Green |
| Bcm_variants_share_the_handler (Baseline, 3 cases) | FOM, FCM, DBM | Own sub type, airline and flight number | Green |
| Missing_source_indicator_names_the_message_type | BSM CHG without `.V/` | No value | Green |

## Error codes and element line (medium item A1, A2)
`ErrorCodes` are `const`. The generic code 1001 (previously used for every element problem) is split by what failed: `1001` length or field count, `1002` value outside the allowed list, `1003` wrong format (digits, letters, characters, shape), `1004` missing or empty field. Specific codes (`V_01`, `F_01`, `S_01` to `S_08`, ...) keep their values. Message level codes (`MSG_<name>_END_INVALID`, `_EMPTY`, `_MISSING_MANDATORY`, `_DUPLICATE`) are built by helper methods in `ErrorCodes`. `ElementValidationResult` keeps the validated element line (`Line`, converted to a string only when read), `ParseError` carries it, and the element error row stores `<description> | <line>` cut to the 250 character column limit (no schema change).
| Test | Input | Expected | Actual |
|---|---|---|---|
| Validators_report_the_kind_of_problem (4 cases) | `.N/022012345`, `.N/022012345600A`, `.W/X/001/20`, `.P/DOE//JOHN` | 1001, 1003, 1002, 1004 | Green |
| Result_carries_the_element_line | Invalid and valid `.N/` lines | `Line` equals the validated line | Green |
| Element_error_row_contains_the_line | BSM with `.F/LH400` | ElementErrors description contains the line | Green |
| Element_error_description_is_truncated | BSM with a 400 character `.R/` line | Description at most 250 characters | Green |
| Codes_are_constants_unique_and_short | Reflection over `ErrorCodes` | All const, unique, at most 50 characters | Green |
| Message_codes_are_centralised | Helper methods | Same strings as before | Green |

## Message header semantics (medium item A3)
IATA table 2.2 names three header concepts: the Standard Message Identifier (BSM, BUM, BCM), the Secondary Level Message Identifier (BAM, FOM, FCM, FFM, DBM, used by BCM) and the Change of Status Indicator (CHG or DEL, used by BSM). `Consts.CHANGE_OF_STATUS` was `["ACK","NAK"]`, so CHG and DEL were stored as a secondary identifier while the handlers described themselves with a change of status. It is now `["CHG","DEL"]`. `MessageHeader.RoutingKey` (identifier + secondary + change of status) is the single routing key, exposed by `IMessageHandler.RoutingKey` and used by `MessageProcessingService`. Rows stored the old way (CHG in the secondary column) build the same key and keep working; no data migration is required.
| Test | Input | Expected | Actual |
|---|---|---|---|
| Second_line_goes_to_the_right_header_field (5 cases) | BSM, BSM CHG, BSM DEL, BCM FOM, BCM DBM | CHG/DEL in change of status, FOM/DBM in secondary identifier | Green |
| Registered_keys_match_the_handlers_own_header | Handler map built by `InstallMessages` | Dictionary key equals `handler.RoutingKey` for every handler | Green |
| Parsed_header_finds_its_handler (8 cases) | Every supported message type | Routing key from stored text exists in the handler map | Green |
| Old_rows_still_route_and_parse | CHG stored in the secondary column | Same routing key, message parses | Green |

## Intake endpoint, spaces, airline designators (medium items A4, A6, K6)
The endpoint answers 202 Accepted with the id of the stored row, rejections use the standard problem details format (400, 413), a message is limited to `Consts.MaxMessageLength` (4000) characters, and the database call honours request cancellation. Authentication stays at the gateway (`SharedPolicy` on `/api/textmessages`). Spaces around header, END or element lines are not tolerated: the endpoint rejects a padded header or END line, the parser rejects padded CHG, END and element lines. Airline designators may contain a digit (`4U`, `9W`, `3K`): `ValidationHelper.IsAirlineDesignator` accepts two alphanumeric characters with at least one letter, or three letters; `ElementIValidator` uses the same helper.
| Test | Input | Expected | Actual |
|---|---|---|---|
| Accepted_response_carries_the_stored_id | Valid BSM | 202 and `id` equal to the stored row id | Green |
| Too_long_message_is_rejected | Message of 4000 + extra characters | 413, nothing stored | Green |
| Message_at_the_limit_is_accepted | Exactly 4000 characters | 202 | Green |
| Bad_request_is_a_problem_details_response | One line message | 400 with `application/problem+json` | Green |
| Padded_envelope_lines_are_rejected (3 cases) | `BSM `, `ENDBSM `, ` BSM` | 400 | Green |
| Padded_lines_are_rejected_by_the_parser (3 cases) | `CHG `, `ENDBSM `, element line with trailing space | Parse fails | Green |
| Valid_flight_identifiers (7 cases) | LH400, EW9021, 9W123, 4U1234, 3K567, A1234, BAW123 | Valid | Green |
| Invalid_flight_identifiers (4 cases) | 12345, L-400, LH40A, LH | Rejected | Green |
| Element_f_splits_a_digit_airline | `.F/9W123/12NOV/JFK/Y` | Airline `9W`, flight `123` | Green |
| Inbound_and_onward_accept_digit_airlines (2 cases) | `.I/9W123/...`, `.O/4U1234/...` | Valid | Green |

## Manual end-to-end check against a real PostgreSQL (2026-11-08)
Not an automated test. A temporary `postgres:17-alpine` container was started, `Aero.TextMessageParser.Migrations` applied `InitialCreate`, the API was started against it and seven messages were posted with `POST /api/textmessages`. Container and process were removed afterwards. This replaces the skipped reproductions for Bug 8 and Bug 9 as evidence; the skipped tests stay skipped because the automated suite has no PostgreSQL.
| Message sent | HTTP | Stored header (identifier / secondary / change of status) | Final status | Error rows |
|---|---|---|---|---|
| BSM with .V .F .N .P | 202 | BSM / - / - | PROCESSED | none |
| BSM CHG, same elements | 202 | BSM / - / CHG | PROCESSED | none |
| BSM DEL with .V .F .N | 202 | BSM / - / DEL | PROCESSED | none |
| BUM with .V .N (handler not implemented) | 202 | BUM / - / - | ERROR, Completed | `MSG_HND_02` NotImplementedException |
| BSM with `.F/LH400` | 202 | BSM / - / - | ERROR, Completed | element error `1001` with the line `.F/LH400` |
| Unknown type XYZ | 202 | XYZ / - / - | ERROR, Completed | `MSG_HND_01` no handler registered |
| BSM ending with ENDBUM | 400 problem details | not stored | - | - |

What this confirms: the claim query with the `"Value"` alias works (Bug 9), a failing message ends in a persisted terminal state (Bug 8), the hosted service starts and resolves its dependencies (Bug 1), CHG and DEL are stored as change of status (A3), element errors carry the failing line (A2), the generated migration creates a model the service can use.

## Baseline (Kind=Baseline, all green)
Single-tag BSM parse; BSM CHG and DEL parse; SplitOnSlash (4 cases); ValidationHelper flight number (5), IATA date (5), airport code (4); Install extensions; model builds with test context; endpoint accepts valid BSM; GetMessageIdentifier happy path; scalar JSON; ElementS valid lengths (5); malformed controls (7).

## Side observation, not tested
`ValidationHelper.ValidateAirlineFlightNumber` requires letters for the first 2 characters when the third is a digit, so airline codes like `4U` (digit + letter) are rejected.

## Manual check of the partial indexes (2026-11-08)
Not an automated test. On a temporary `postgres:17-alpine` container both migrations were applied, 100000 rows were inserted (100 `NOT_PROCESSED`, the rest `PROCESSED`) and the claim query was explained. The plan is an `Index Scan using IX_TextMessages_Pending` without a Sort node; the index is 16 kB while the table index on the primary key is 2208 kB. `Completed` is no longer a column, it is computed from `Status`.
