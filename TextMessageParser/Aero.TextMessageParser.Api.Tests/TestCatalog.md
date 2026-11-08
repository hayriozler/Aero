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

## Bug 3 - Stateful shared element instances (RED)
| Test | Input | Expected | Actual | Why it matters |
|---|---|---|---|---|
| Three_n_lines_give_three_tags | BSM with `.N/0220123456001`, `.N/0220123457001`, `.N/0220123458001` | Tags 0220123456, 0220123457, 0220123458 | `[0220123458, 0220123458, 0220123458]` | One ElementN instance is reused; the DTO is built after all parsing, so every result shows the last line. Multi-bag passengers get the wrong tags. |
| Two_o_lines_give_two_distinct_onwards | `.O/LH401/13NOV/MUC/Y`, `.O/LH402/14NOV/BER/Y` | Destinations MUC, BER | `[BER, BER]` | Same shared-instance problem for ElementO. |
| Element_p_state_does_not_leak_between_messages | A: `.P/2DOE/JOHN/MIDDLE1/MIDDLE2`, then B: `.P/ROE` | B state: no additional names, no count, no given name | AdditionalGivenNames still has MIDDLE1, MIDDLE2 (list only appended) | Data of one passenger leaks into the next message. |
| Passenger_name_does_not_leak_into_dto | A: `.P/DOE/JOHN`, B: `.P/ROE` | B name without JOHN | `JOHN ROE` | Leak visible in the DTO. |
| Element_s_seat_does_not_leak (probe) | A: `.S/Y/12A/B/001/SEC123/Y/N/A`, B: `.S/Y` | B seat is null | null (green) | NOT a bug: ElementS assigns every property on each Parse. |
| Element_w_state_does_not_leak | A: `.W/K/001/20/CM/55/40/20/XYZ`, B: `.W/K/001/20` | Unit and LengthOfBag empty | Unit `CM` leaked | Unit/dimensions are only assigned inside one branch. |
| Concurrent_processing_does_not_mix_results | 32 threads x 20 rounds, unique serial and name each, one shared Bsm, Barrier-synchronised | Each DTO matches its own message | ~250-320 of 640 mixed (e.g. msg 28 got tag of msg 27) | Real concurrent processing corrupts data. (Count varies run to run.) |

## Bug 4 - Validators skip non-Mandatory elements (RED)
| Test | Input | Expected | Actual |
|---|---|---|---|
| Malformed_element_is_rejected_when_mandatory (control, Baseline) | `.F/ .I/ .O/ .P/ .S/ .U/ .W/` as Mandatory | Rejected | Rejected (green); proves the samples are malformed |
| Malformed_element_is_rejected_regardless_of_requirement | The same 7 lines with Conditional and with Optional (14 cases) | Rejected | All 14 accepted (red) because every validator starts with `if (Requirement != Mandatory) return valid` |
| Malformed_conditional_f_gives_error_not_exception | BSM with `.F/`, `.F/LH400`, `.F/LH400/12NOV`, `.F/XX/YY/ZZ/Q` | `ProcessMessage` returns null and an error row is recorded | IndexOutOfRangeException from ElementF.Parse (4 cases red) |
| Malformed_conditional_f_currently_throws | BSM with `.F/` | Documents the exception type | IndexOutOfRangeException (green, documents current behavior) |

## Bug 5 - ElementSValidator (RED)
| Test | Input | Expected | Actual |
|---|---|---|---|
| ElementS_valid_values_pass | `.S/B`, `.S/Y/12A/C/Y`, `.S/Y/12A/C/001/I`, `.S/Y/12A/C/001/A` (Mandatory) | Valid | All rejected (S_01, S_02, S_03): `if (list.Contains(x)) AddError` is inverted |
| ElementS_invalid_values_fail | `.S/Q`, `.S/Y/12A/C/Q`, `.S/Y/12A/C/001/Z` | Invalid | All accepted |
| ElementS_unchecked_lengths_reject_invalid_values | 3 fields `.S/Q/12A`; 4 `.S/Y/12A/Q`; 7 `...SEC/Q`; 8 `...SEC/Y/Q`; 9 `...SEC/Y/N/Z` | Invalid | All accepted: switch only covers 2, 5, 6 parts |
| ElementS_valid_lengths_pass (control, Baseline) | Valid S with 3, 4, 7, 8, 9 parts | Valid | Valid (green) |

Additional observation: the validator's field positions for the 2/5/6-part branches (fields 1, 4, 5) disagree with `ElementS.Parse` (status at 3, profile at 6, bag tag status at 8). The tests for the 2/5/6 branches follow the validator's own indices; the 3/4/7/8/9 tests follow the parser layout. The field mapping needs a decision from the owner.

## Bug 6 - Bsm/BsmChg mapping (RED)
| Test | Input | Expected | Actual |
|---|---|---|---|
| Bsm_passenger_status_mapping | `.S/Y/12A/B/001/SEC123/Y/N/A` (status B, profile Y) | DTO.PassengerStatus = `B` | `Y` (profile status is passed) |
| BsmChg_passenger_status_mapping | Same, in BSM CHG | `B` | `Y` |
| Dto_exposes_weight (2 DTO types) | `.W/K/001/20` | A weight property exists on the DTO | No weight property; PiecesWeightIndicator/CheckedWeight go to unused locals, W data is lost |

## Bug 7 - Unknown element lines (RED)
| Test | Input | Expected | Actual |
|---|---|---|---|
| Unknown_element_gives_validation_error | Extra line `.Z/XYZ`, `THIS IS FREE TEXT`, `AB`, `.U/CONT1/1/M/Y/FRA/N` (U is not in BSM) | null result and error code `UNKNOWN_ELEMENT_FOUND` | InvalidOperationException "Sequence contains no matching element" (4 red); the UNKNOWN_ELEMENT_FOUND branch is unreachable |
| Split_unknown_currently_throws | `SplitSpanMessage` with `.Z/XYZ` | Documents the exception | InvalidOperationException (green) |

## Bug 8 - Processing failure handling
| Test | Result |
|---|---|
| Failed_message_is_persisted_as_terminal | SKIPPED: needs PostgreSQL (raw claim SQL, private method). Skip reason holds the reproduction: BUM message, observe Status stays PROCESSING, Completed false. |
| Catch_block_persists_failure (SourceGuard) | RED: the per-message catch block only sets `msg.Status = "ERROR"` in memory (no Completed, no SaveChanges). |
| Bum_handler_throws_not_implemented | Green, documents that `Bum.Handle` throws NotImplementedException (what reaches the catch block). |

## Bug 9 - Raw SQL alias
| Test | Result |
|---|---|
| Raw_sql_returns_value_column | SKIPPED: needs PostgreSQL. Reason documents the expected EF error (column Value missing). |
| Claim_sql_aliases_id_as_value (SourceGuard) | RED: SQL ends with `RETURNING "Id"` without `AS "Value"`. |

## Bug 10 - JSON serialization of Onwards (RED)
| Test | Input | Expected | Actual |
|---|---|---|---|
| Request_onwards_are_serialized (default and web options) | Onwards `("LH","401","13NOV","MUC",'Y')`, `("EW","9021","14NOV","BER",'C')` in TextMessageDepartureBagRequest | JSON contains airline, number, date, destination of each | Each onward is `{}` (ValueTuple fields are ignored) |
| Chg_onwards_are_serialized (default and web) | Same in TextMessageDepartureBagChgDto | Same | `{}` |
| Onwards_currently_serialize_as_empty_objects | Same | Documents `{}` | `{}` (green) |
| Scalars_are_serialized (Baseline) | Scalars and bag tags | Present | Green |

## Bug 11 - BCM subtype
| Test | Input | Expected | Actual |
|---|---|---|---|
| Bcm_subtype FOM | `BCM / FOM / .V/7LFRA / .F/LH400/12NOV/JFK / ENDBCM` | `Fom` | `Fom` (green) |
| Bcm_subtype FCM | same with FCM | `Fcm` | `Fcm` (green) |
| Bcm_subtype DBM | same with DBM | `Dbm` | `Fcm` (RED, hard-coded in BcmDbm) |

## Bug 12 - Envelope handling (RED)
| Test | Input | Expected | Actual |
|---|---|---|---|
| GetMessageIdentifier_degenerate_input_does_not_throw_index_exceptions | `null`, `""`, `"BSM"`, 9 spaces, `"\r\n\r\n\r\n"` | No index exception | ArgumentOutOfRangeException for all 5 inputs (5 red) |
| GetMessageIdentifier_one_line_message_currently_throws | `"BSM"` | Documents | ArgumentOutOfRangeException (green) |
| Bsm_with_mismatching_end_line_is_rejected | BSM ending with `ENDBUM` | null + error row | Accepted |
| Bsm_without_end_line_is_rejected | BSM without any END line | null + error row | Accepted; `.P/DOE/JOHN` silently becomes the footer |
| Endpoint_returns_bad_request_for_degenerate_bodies | POST `{"message":null}`, `{}`, 9 spaces, `BSM-------` | 400 | 500 (exception middleware) |
| Endpoint_rejects_bad_footer | POST BSM with ENDBUM / without END | 400 | 200 (stored) |
| Endpoint_accepts_valid_bsm (Baseline) | Sample BSM | 200 and one row stored | Green |
| GetMessageIdentifier_reads_header_and_footer (Baseline) | BSM, BSM CHG, BSM DEL | Header, secondary identifier, footer | Green |

## Bug 13 - Newline handling (RED on Windows)
| Test | Input | Expected | Actual (Windows) |
|---|---|---|---|
| GetMessageIdentifier_is_independent_of_line_ending LF | Sample BSM with `\n` | Header BSM, footer ENDBSM | ArgumentOutOfRangeException (`Span.Split(char[])` splits on the whole `\r\n` sequence, so LF-only text is one line) |
| same, CRLF | with `\r\n` | same | Green |
| ProcessMessage_is_independent_of_line_ending LF / CRLF | Sample BSM | Same DTO | LF red, CRLF green |
| NewLineSeparator_is_cached (Priority=Low) | Two reads of `Consts.NEW_LINE_SEPERATOR` | Same array instance | New array each time (red) |

On Linux the separator is `['\n']`, so CRLF text would keep a trailing `\r` on every line; that direction cannot be exercised on this machine.

## Bug 14 - Parsed result storage (RED)
Model_contains_parsed_result_storage: expects at least one non-owned entity from the Api assembly besides TextMessage, TextMessageError, ElementError. Actual: none. The schema decision belongs to the owner.

## Bug 15 - Additional finding: production DbContext model cannot be built (RED)
Production_model_builds: `new TextMessageParserDbContext(options).Model` throws "The entity type 'Permission' requires a primary key". `DataStoreContext` has DbSets for User, Role, Permission and so on but applies configurations only from the Api assembly. Any real DbContext use (endpoint save, processing service) fails. Tests use `TestParserDbContext` to work around it.

## Baseline (Kind=Baseline, all green)
Single-tag BSM parse; BSM CHG and DEL parse; SplitOnSlash (4 cases); ValidationHelper flight number (5), IATA date (5), airport code (4); Install extensions; model builds with test context; endpoint accepts valid BSM; GetMessageIdentifier happy path; scalar JSON; ElementS valid lengths (5); malformed controls (7).

## Side observation, not tested
`ValidationHelper.ValidateAirlineFlightNumber` requires letters for the first 2 characters when the third is a digit, so airline codes like `4U` (digit + letter) are rejected.
