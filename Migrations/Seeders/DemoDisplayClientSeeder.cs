using Aero.Fids.Infrastructure.Boards;
using Aero.Fids.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Aero.Migrations.Seeders;

// Debug-only demo data — Terminal1/Terminal2 FIDS display clients (monitors) for local testing.
// Resource-type monitors (Counter/Gate/*Belt) get their ResourceCode from AeroFlightDbContext's own
// Counter/Gate/DepartureBelt/ArrivalBelt tables — never hardcoded. Counter/Gate/Belt carry no real
// Terminal column, so "this terminal's resources" means codes matching DemoResourceSeeder's naming
// prefix; if a terminal doesn't have enough of its own (that seeder didn't run, or ran short), the
// remaining slots are filled with a random real code of that same type instead of being left unset.
internal sealed class DemoDisplayClientSeeder(FidsDbContext db, ILogger<DemoDisplayClientSeeder> logger)
{
    private sealed record Monitor(string Name, string Type, string? ResourceCode, string Location, string? BankName, int? BankPosition);

    private static readonly Monitor[] _monitors =
    [
        new("Ter1_OutSide", DisplayClientTypes.DepartureFlightList, null, "Terminal 1", null, null),
        new("Ter1_Dep_Salon_01", DisplayClientTypes.DepartureFlightList, null, "Terminal 1", "Ter1_Dep_Salon", 1),
        new("Ter1_Dep_Salon_02", DisplayClientTypes.DepartureFlightList, null, "Terminal 1", "Ter1_Dep_Salon", 2),
        new("Ter1_Dep_Salon_03", DisplayClientTypes.DepartureFlightList, null, "Terminal 1", "Ter1_Dep_Salon", 3),
        new("Ter1_Arr_Salon_01", DisplayClientTypes.ArrivalFlightList, null, "Terminal 1", "Ter1_Arr_Salon", 1),
        new("Ter1_Arr_Salon_02", DisplayClientTypes.ArrivalFlightList, null, "Terminal 1", "Ter1_Arr_Salon", 2),

        new("Ter2_OutSide", DisplayClientTypes.DepartureFlightList, null, "Terminal 2", null, null),
        new("Ter2_Dep_Salon", DisplayClientTypes.DepartureFlightList, null, "Terminal 2", null, null),

        new("Ter1_Full_Board", DisplayClientTypes.FullFlightList, null, "Terminal 1", null, null),
        new("Cnt_Ter1_01", DisplayClientTypes.Counter, "102", "Terminal 1", null, null),
        new("Cnt_Ter1_02", DisplayClientTypes.Counter, "103", "Terminal 1", null, null),
        new("Cnt_Ter1_03", DisplayClientTypes.Counter, "104", "Terminal 1", null, null),
        new("Cnt_Ter1_04", DisplayClientTypes.Counter, "105", "Terminal 1", null, null),
        new("Gt_Ter1_01", DisplayClientTypes.Gate, "08", "Terminal 1", null, null),
        new("Gt_Ter1_02", DisplayClientTypes.Gate, "09", "Terminal 1", null, null),
        new("ArrBelt_Ter1_02", DisplayClientTypes.ArrivalBelt, "01", "Terminal 1", null, null),
        new("Cnt_Ter2_01", DisplayClientTypes.Counter, "202", "Terminal 2", null, null),
        new("Cnt_Ter2_02", DisplayClientTypes.Counter, "203", "Terminal 2", null, null),
        new("Gt_Ter2_01", DisplayClientTypes.Gate, "208", "Terminal 2", null, null),

        // "T1"/"T2"/"D" are real codes in parkingpositions.csv (the "Imaginary Park Position" rows) —
        // used here as-is since real numbered stands aren't tied to one specific terminal.
        new("Spot_T1", DisplayClientTypes.Spot, "T1", "Terminal 1", null, null),
        new("Spot_T2", DisplayClientTypes.Spot, "T2", "Terminal 2", null, null),
        new("Spot_D", DisplayClientTypes.Spot, "D", "Domestic", null, null),
    ];

    public async Task SeedAsync(CancellationToken ct)
    {
        var existing = await db.DisplayClientSet.ToDictionaryAsync(d => d.Name, StringComparer.Ordinal, ct);
        var now = DateTimeOffset.UtcNow;
        var added = 0;
        var updated = 0;

        foreach (var m in _monitors)
        {
            if (existing.TryGetValue(m.Name, out var current))
            {
                if (current.Type == m.Type && current.ResourceCode == m.ResourceCode && current.Location == m.Location &&
                    current.BankName == m.BankName && current.BankPosition == m.BankPosition)
                    continue;

                current.Update(m.Name, m.Type, m.ResourceCode, m.Location, m.BankName, m.BankPosition, current.IsActive, now);
                updated++;
            }
            else
            {
                db.DisplayClientSet.Add(DisplayClient.Create(m.Name, m.Type, m.ResourceCode, m.Location, m.BankName, m.BankPosition, now));
                added++;
            }
        }

        if (added == 0 && updated == 0) return;
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Added} new and updated {Updated} existing demo display clients.", added, updated);
    }
}
