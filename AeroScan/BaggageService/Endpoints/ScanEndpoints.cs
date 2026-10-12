namespace BaggageService.Endpoints;

//public static class ScanEndpoints
//{
//    public static IEndpointRouteBuilder MapScanEndpoints(this IEndpointRouteBuilder app)
//    {
//        var scans = app.MapGroup("/api/v1/scans").RequireAuthorization();

//        scans.MapPost("/", SubmitScan)
//            .WithName("SubmitScan")
//            .Produces<ScanRecord>(201)
//            .ProducesProblem(400);

//        scans.MapPost("/batch", SubmitBatch)
//            .WithName("SubmitBatch")
//            .Produces(200);

//        scans.MapGet("/{flightKey}", GetFlightScans)
//            .WithName("GetFlightScans")
//            .Produces<IReadOnlyList<ScanRecord>>();

//        scans.MapGet("/device/{deviceId}/status", GetDeviceStatus)
//            .WithName("GetDeviceStatus")
//            .Produces<DeviceStatus>()
//            .ProducesProblem(404);

//        return app;
//    }

//    private static async Task<Results<Created<ScanRecord>, BadRequest<string>>> SubmitScan(
//        SubmitScanRequest request,
//        AeroScanDataContext db,
//        CancellationToken ct)
//    {
//        var record = new ScanRecord
//        {
//            DeviceId = request.DeviceId,
//            TagNumber = request.TagNumber,
//            FlightKey = request.FlightKey,
//            LocationCode = request.LocationCode,
//            ScanType = request.ScanType,
//            OperatorId = request.OperatorId,
//            DeviceTimestamp = request.DeviceTimestamp,
//            IsOffline = request.IsOffline,
//            Latitude = request.Latitude,
//            Longitude = request.Longitude
//        };

//        db.ScanRecordSet.Add(record);
//        await db.SaveChangesAsync(ct);
      

//        record.IsPublished = true;
//        await db.SaveChangesAsync(ct);

//        return TypedResults.Created($"/api/v1/scans/{record.Id}", record);
//    }

//    private static async Task<Ok> SubmitBatch(
//        IReadOnlyList<SubmitScanRequest> requests,
//        AeroScanDataContext db,
//        CancellationToken ct)
//    {
//        // Sort by device timestamp to handle late offline scans in order
//        var ordered = requests.OrderBy(r => r.DeviceTimestamp).ToList();

//        foreach (var request in ordered)
//        {
//            var record = new ScanRecord
//            {
//                DeviceId = request.DeviceId,
//                TagNumber = request.TagNumber,
//                FlightKey = request.FlightKey,
//                LocationCode = request.LocationCode,
//                ScanType = request.ScanType,
//                OperatorId = request.OperatorId,
//                DeviceTimestamp = request.DeviceTimestamp,
//                IsOffline = true,
//                Latitude = request.Latitude,
//                Longitude = request.Longitude
//            };
//            db.ScanRecordSet.Add(record);

            
//            record.IsPublished = true;
//        }

//        await db.SaveChangesAsync(ct);
//        return TypedResults.Ok();
//    }

//    private static async Task<Ok<IReadOnlyList<ScanRecord>>> GetFlightScans(
//        string flightKey,
//        AeroScanDataContext db,
//        CancellationToken ct)
//    {
//        var records = await db.ScanRecordSet
//            .Where(s => s.FlightKey == flightKey)
//            .OrderBy(s => s.ServerTimestamp)
//            .ToListAsync(ct);

//        return TypedResults.Ok<IReadOnlyList<ScanRecord>>(records);
//    }

//    private static async Task<Results<Ok<DeviceStatus>, NotFound>> GetDeviceStatus(
//        string deviceId,
//        AeroScanDataContext db,
//        CancellationToken ct)
//    {
//        var status = await db.DeviceStatuseSet
//            .FirstOrDefaultAsync(d => d.DeviceId == deviceId, ct);

//        return status is null ? TypedResults.NotFound() : TypedResults.Ok(status);
//    }
//}
