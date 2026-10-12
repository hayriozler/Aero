namespace BaggageService.Endpoints;

//public static class ReconciliationEndpoints
//{
//    public static IEndpointRouteBuilder MapReconciliationEndpoints(this IEndpointRouteBuilder app)
//    {
//        var recon = app.MapGroup("/api/v1/reconciliation").RequireAuthorization();

//        recon.MapGet("/{flightKey}", GetReconciliation)
//            .WithName("GetReconciliation")
//            .Produces<ReconciliationDto>()
//            .ProducesProblem(404);

//        recon.MapPost("/{flightKey}/close", CloseReconciliation)
//            .WithName("CloseReconciliation")
//            .Produces<ReconciliationDto>()
//            .ProducesProblem(400)
//            .ProducesProblem(404)
//            .RequireAuthorization(Policies.Supervisor);

//        return app;
//    }

//    private static async Task<Results<Ok<ReconciliationDto>, NotFound>> GetReconciliation(
//        string flightKey,
//        AeroScanDataContext db,
//        CancellationToken ct)
//    {
//        if (!FlightKey.TryParse(flightKey, out var key))
//            return TypedResults.NotFound();

//        var record = await db.ReconciliationRecordSet
//            .FirstOrDefaultAsync(r => r.FlightKey == key, ct);

//        return record is null ? TypedResults.NotFound() : TypedResults.Ok(record.ToDto());
//    }

//    private static async Task<Results<Ok<ReconciliationDto>, NotFound, BadRequest<string>>> CloseReconciliation(
//        string flightKey,
//        string operatorId,
//        AeroScanDataContext db,
//        CancellationToken ct)
//    {
//        if (!FlightKey.TryParse(flightKey, out var key))
//            return TypedResults.NotFound();

//        var record = await db.ReconciliationRecordSet
//            .FirstOrDefaultAsync(r => r.FlightKey == key, ct);

//        if (record is null) return TypedResults.NotFound();

//        var result = record.Close(operatorId);
//        if (result.IsFailure)
//            return TypedResults.BadRequest(result.Error!);

//        await db.SaveChangesAsync(ct);
        

//        return TypedResults.Ok(record.ToDto());
//    }
//}
