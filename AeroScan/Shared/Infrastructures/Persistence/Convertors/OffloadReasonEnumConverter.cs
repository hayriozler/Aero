using Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.Convertors;

public class OffloadReasonEnumConverter : ValueConverter<OffloadReason, string>
{
  public OffloadReasonEnumConverter() : base(
        v => ToString(v).ToString(),
        v => FromString(v))
    { }
  
    private static string ToString(OffloadReason status) => status switch
    {
        OffloadReason.PassengerNoShow => "NS",
        OffloadReason.SecurityInstruction => "SI",
        OffloadReason.WeightAndBalance => "WB",
        OffloadReason.DangerousGoods => "DG",
        OffloadReason.ManualOverride => "MO",
        _ => throw new Exception($"Invalid status: {status}")
    };

    private static OffloadReason FromString(string c) => c switch
    {
        "NS" => OffloadReason.PassengerNoShow,
        "SI" => OffloadReason.SecurityInstruction,
        "WB" => OffloadReason.WeightAndBalance,
        "DG" => OffloadReason.DangerousGoods,
        "MO" => OffloadReason.ManualOverride,
        _ => throw new Exception($"Invalid status: {c}")
    };
}
