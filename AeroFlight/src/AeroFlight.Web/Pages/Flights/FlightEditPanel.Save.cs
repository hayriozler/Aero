using AeroFlight.Web.Services;
using AeroFlight.Web.Services.Flights;
using MudBlazor;

namespace AeroFlight.Web.Pages.Flights;

public partial class FlightEditPanel
{
    // Adding/editing/deleting a routing leg recomputes CountryType server-side — refresh just that
    // (and the routing summary) without reloading the whole form, so in-progress edits on other
    // fields aren't clobbered by a full LoadAsync().
    private async Task RefreshCountryTypeAsync()
    {
        if (Id is null)
            return;

        try
        {
            if (_isArrival)
            {
                var d = await Flights.GetArrivalDetailAsync(Id.Value);
                _model.CountryType = d.CountryType ?? "";
                _model.RoutingSummary = d.RoutingSummary;
            }
            else
            {
                var d = await Flights.GetDepartureDetailAsync(Id.Value);
                _model.CountryType = d.CountryType ?? "";
                _model.RoutingSummary = d.RoutingSummary;
            }
        }
        catch (ApiException ex)
        {
            Snackbar.Add($"Failed to refresh International/Domestic: {ex.Message}", Severity.Error);
        }
        catch (Exception)
        {
            Snackbar.Add("Could not reach the API while refreshing International/Domestic.", Severity.Error);
        }
    }

    private async Task LoadAsync()
    {
        _loading = true;
        try
        {
            await FetchAndPopulateAsync();
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task RefreshAsync()
    {
        _refreshing = true;
        try
        {
            await FetchAndPopulateAsync();
            if (!_notFound)
                Snackbar.Add("Flight refreshed.", Severity.Success);
        }
        finally
        {
            _refreshing = false;
        }
    }

    private async Task FetchAndPopulateAsync()
    {
        _notFound = false;
        try
        {
            if (_isArrival)
            {
                _arrival = await Flights.GetArrivalDetailAsync(Id!.Value);
                _departure = null;
                PopulateFromArrival(_arrival);
            }
            else
            {
                _departure = await Flights.GetDepartureDetailAsync(Id!.Value);
                _arrival = null;
                PopulateFromDeparture(_departure);
            }
        }
        catch (ApiException ex) when (ex.StatusCode == 404)
        {
            _notFound = true;
        }
        catch (ApiException ex)
        {
            Snackbar.Add($"Failed to load flight: {ex.Message}", Severity.Error);
            _notFound = true;
        }
        catch (Exception)
        {
            Snackbar.Add("Could not reach the API while loading the flight.", Severity.Error);
            _notFound = true;
        }
    }

    private void PopulateFromArrival(ArrivalFlightDetail d)
    {
        _title = $"{d.AirlineCode}/{d.FlightNumber}";
        _airlineCode = d.AirlineCode;
        _flightDate = d.FlightDate;
        _linkedFlightId = d.DepartureFlightId;

        _model.FlightNumber = d.FlightNumber;
        _model.StatusCode = d.Status;

        var local = TenantTimeZone.ToLocal(d.Sta, _tenantTimeZone);
        _model.ScheduledDate = local.Date;
        _model.ScheduledTime = local.TimeOfDay;
        _model.AirportCode = d.OriginAirportCode;
        _model.CallSign = d.CallSign ?? "";
        _model.OperatingAirlineCode = d.OperatingAirlineCode ?? "";
        _model.FlightTypeCode = d.FlightTypeCode ?? "";
        _model.AircraftCode = d.AircraftCode ?? "";
        _model.AircraftTypeCode = d.AircraftTypeCode ?? "";
        _model.TerminalCode = d.TerminalCode ?? "";
        _model.HandlingAgentCode = d.HandlingAgentCode ?? "";
        _model.Comment = d.Comment ?? "";
        _model.PaxTotal = d.PaxTotal;
        _model.CodeshareNumber = d.CodeshareNumber;
        _model.CountryType = d.CountryType ?? "";
        _model.ParkPositionCode = d.ParkPositionCode ?? "";
        _model.RoutingSummary = d.RoutingSummary;

        _model.DivertAirportCode = d.DivertAirportCode ?? "";

        (_model.EtaDate, _model.EtaTime) = ToLocalPair(d.Eta);
        (_model.AtaDate, _model.AtaTime) = ToLocalPair(d.Ata);
        (_model.OnBlockDate, _model.OnBlockTime) = ToLocalPair(d.OnBlock);

        _model.PaxTransit = d.LoadInfo.PaxTransit;
        _model.PaxAssistanceRequired = d.LoadInfo.PaxAssistanceRequired;
        _model.Pax1 = d.LoadInfo.Pax1;
        _model.Pax2 = d.LoadInfo.Pax2;
        _model.Pax3 = d.LoadInfo.Pax3;
        _model.CrewNumber = d.LoadInfo.CrewNumber;
        _model.CrewCockpit = d.LoadInfo.CrewCockpit;
        _model.BaggageWeight = d.LoadInfo.BaggageWeight;
        _model.BaggageTotal = d.LoadInfo.BaggageTotal;
        _model.CargoWeight = d.LoadInfo.CargoWeight;
        _model.MailWeight = d.LoadInfo.MailWeight;
        _model.TotalWeight = d.LoadInfo.TotalWeight;
        _model.EquipmentWeight = d.LoadInfo.EquipmentWeight;
        _model.CabinLuggage = d.LoadInfo.CabinLuggage;
    }

    private void PopulateFromDeparture(DepartureFlightDetail d)
    {
        _title = $"{d.AirlineCode}/{d.FlightNumber}";
        _airlineCode = d.AirlineCode;
        _flightDate = d.FlightDate;
        _linkedFlightId = d.ArrivalFlightId;

        _model.FlightNumber = d.FlightNumber;
        _model.StatusCode = d.Status;

        var local = TenantTimeZone.ToLocal(d.Std, _tenantTimeZone);
        _model.ScheduledDate = local.Date;
        _model.ScheduledTime = local.TimeOfDay;
        _model.AirportCode = d.DestinationAirportCode;
        _model.CallSign = d.CallSign ?? "";
        _model.OperatingAirlineCode = d.OperatingAirlineCode ?? "";
        _model.FlightTypeCode = d.FlightTypeCode ?? "";
        _model.AircraftCode = d.AircraftCode ?? "";
        _model.AircraftTypeCode = d.AircraftTypeCode ?? "";
        _model.TerminalCode = d.TerminalCode ?? "";
        _model.HandlingAgentCode = d.HandlingAgentCode ?? "";
        _model.Comment = d.Comment ?? "";
        _model.PaxTotal = d.PaxTotal;
        _model.CodeshareNumber = d.CodeshareNumber;
        _model.CountryType = d.CountryType ?? "";
        _model.ParkPositionCode = d.ParkPositionCode ?? "";
        _model.RoutingSummary = d.RoutingSummary;

        (_model.EtdDate, _model.EtdTime) = ToLocalPair(d.Etd);
        (_model.AtdDate, _model.AtdTime) = ToLocalPair(d.Atd);
        (_model.OffBlockDate, _model.OffBlockTime) = ToLocalPair(d.OffBlock);

        _model.PaxTransit = d.LoadInfo.PaxTransit;
        _model.PaxAssistanceRequired = d.LoadInfo.PaxAssistanceRequired;
        _model.Pax1 = d.LoadInfo.Pax1;
        _model.Pax2 = d.LoadInfo.Pax2;
        _model.Pax3 = d.LoadInfo.Pax3;
        _model.CrewNumber = d.LoadInfo.CrewNumber;
        _model.CrewCockpit = d.LoadInfo.CrewCockpit;
        _model.BaggageWeight = d.LoadInfo.BaggageWeight;
        _model.BaggageTotal = d.LoadInfo.BaggageTotal;
        _model.CargoWeight = d.LoadInfo.CargoWeight;
        _model.MailWeight = d.LoadInfo.MailWeight;
        _model.TotalWeight = d.LoadInfo.TotalWeight;
        _model.EquipmentWeight = d.LoadInfo.EquipmentWeight;
        _model.CabinLuggage = d.LoadInfo.CabinLuggage;
    }

    private async Task SaveAsync()
    {
        await _form.ValidateAsync();
        if (Id is null)
            return;
        if (!_form.IsValid)
        {
            Snackbar.Add("Fix the highlighted fields before saving.", Severity.Warning);
            return;
        }

        var localDate = _model.ScheduledDate ?? DateTime.Today;
        var localTime = _model.ScheduledTime ?? TimeSpan.Zero;
        var scheduled = TenantTimeZone.ToUtcOffset(localDate.Date + localTime, _tenantTimeZone);

        _saving = true;
        try
        {
            FlightResponse response;
            if (_isArrival)
            {
                var request = new UpdateArrivalRequest(
                    scheduled,
                    _model.AirportCode,
                    _model.FlightTypeCode,
                    _model.FlightNumber,
                    _model.StatusCode,
                    Blank(_model.CallSign),
                    Blank(_model.OperatingAirlineCode),
                    Blank(_model.AircraftCode),
                    Blank(_model.AircraftTypeCode),
                    Blank(_model.TerminalCode),
                    Blank(_model.HandlingAgentCode),
                    Blank(_model.Comment),
                    _model.PaxTotal,
                    _model.CodeshareNumber,
                    Blank(_model.DivertAirportCode),
                    ToUtcOffsetOrNull(_model.EtaDate, _model.EtaTime),
                    ToUtcOffsetOrNull(_model.AtaDate, _model.AtaTime),
                    null, // TouchDown — not yet editable from this screen
                    ToUtcOffsetOrNull(_model.OnBlockDate, _model.OnBlockTime),
                    _model.PaxTransit,
                    _model.PaxAssistanceRequired,
                    _model.Pax1,
                    _model.Pax2,
                    _model.Pax3,
                    _model.CrewNumber,
                    _model.CrewCockpit,
                    _model.BaggageWeight,
                    _model.BaggageTotal,
                    _model.CargoWeight,
                    _model.MailWeight,
                    _model.TotalWeight,
                    _model.EquipmentWeight,
                    _model.CabinLuggage,
                    Blank(_model.ParkPositionCode));
                response = await Flights.UpdateArrivalAsync(Id.Value, request);
            }
            else
            {
                var request = new UpdateDepartureRequest(
                    scheduled,
                    _model.AirportCode,
                    _model.FlightTypeCode,
                    _model.FlightNumber,
                    _model.StatusCode,
                    Blank(_model.CallSign),
                    Blank(_model.OperatingAirlineCode),
                    Blank(_model.AircraftCode),
                    Blank(_model.AircraftTypeCode),
                    Blank(_model.TerminalCode),
                    Blank(_model.HandlingAgentCode),
                    Blank(_model.Comment),
                    _model.PaxTotal,
                    _model.CodeshareNumber,
                    ToUtcOffsetOrNull(_model.EtdDate, _model.EtdTime),
                    ToUtcOffsetOrNull(_model.AtdDate, _model.AtdTime),
                    ToUtcOffsetOrNull(_model.OffBlockDate, _model.OffBlockTime),
                    null, // TakeOff — not yet editable from this screen
                    _model.PaxTransit,
                    _model.PaxAssistanceRequired,
                    _model.Pax1,
                    _model.Pax2,
                    _model.Pax3,
                    _model.CrewNumber,
                    _model.CrewCockpit,
                    _model.BaggageWeight,
                    _model.BaggageTotal,
                    _model.CargoWeight,
                    _model.MailWeight,
                    _model.TotalWeight,
                    _model.EquipmentWeight,
                    _model.CabinLuggage,
                    Blank(_model.ParkPositionCode));
                response = await Flights.UpdateDepartureAsync(Id.Value, request);
            }

            Snackbar.Add($"Flight {response.AirlineCode}{response.FlightNumber} saved.", Severity.Success);
            await LoadAsync();
        }
        catch (ApiException ex)
        {
            Snackbar.Add($"Failed to save flight: {ex.Message}", Severity.Error);
        }
        catch (Exception)
        {
            Snackbar.Add("Could not reach the API while saving the flight.", Severity.Error);
        }
        finally
        {
            _saving = false;
        }
    }

    private async Task DeleteAsync()
    {
        if (Id is null)
            return;

        var deleteLinkedFlight = false;

        if (_linkedFlightId is { } linkedId)
        {
            var linkedKind = _isArrival ? "Departure" : "Arrival";
            var choice = await DialogService.ShowMessageBoxAsync(
                "Delete Flight",
                $"{_title ?? "This flight"} is linked to {linkedKind} flight #{linkedId}. " +
                "Delete only this flight, or both linked flights? This cannot be undone.",
                yesText: "Delete Both", noText: "Delete This Only", cancelText: "Cancel");
            if (choice is null)
                return;

            deleteLinkedFlight = choice == true;
        }
        else
        {
            var confirmed = await DialogService.ShowMessageBoxAsync(
                "Delete Flight", $"Permanently delete {_title ?? "this flight"}? This cannot be undone.",
                yesText: "Delete", cancelText: "Cancel");
            if (confirmed != true)
                return;
        }

        _deleting = true;
        try
        {
            if (_isArrival)
                await Flights.DeleteArrivalAsync(Id.Value, deleteLinkedFlight);
            else
                await Flights.DeleteDepartureAsync(Id.Value, deleteLinkedFlight);

            Snackbar.Add("Flight deleted.", Severity.Success);
            if (OnDeleted.HasDelegate)
                await OnDeleted.InvokeAsync();

            if (OnClose.HasDelegate)
                await OnClose.InvokeAsync();
            else
                Navigation.NavigateTo("/flights");
        }
        catch (ApiException ex)
        {
            Snackbar.Add($"Failed to delete flight: {ex.Message}", Severity.Error);
        }
        catch (Exception)
        {
            Snackbar.Add("Could not reach the API while deleting the flight.", Severity.Error);
        }
        finally
        {
            _deleting = false;
        }
    }

    private async Task UnlinkAsync()
    {
        if (Id is null || _linkedFlightId is not { } linkedId)
            return;

        var linkedKind = _isArrival ? "Departure" : "Arrival";
        var confirmed = await DialogService.ShowMessageBoxAsync(
            "Unlink Flight", $"Unlink {_title ?? "this flight"} from {linkedKind} flight #{linkedId}? Neither flight is deleted.",
            yesText: "Unlink", cancelText: "Cancel");
        if (confirmed != true)
            return;

        try
        {
            if (_isArrival)
                await Flights.UnlinkArrivalAsync(Id.Value);
            else
                await Flights.UnlinkDepartureAsync(Id.Value);

            _linkedFlightId = null;
            Snackbar.Add("Flights unlinked.", Severity.Success);

            if (OnLinkChanged.HasDelegate)
                await OnLinkChanged.InvokeAsync();
        }
        catch (ApiException ex)
        {
            Snackbar.Add($"Failed to unlink flight: {ex.Message}", Severity.Error);
        }
        catch (Exception)
        {
            Snackbar.Add("Could not reach the API while unlinking the flight.", Severity.Error);
        }
    }

    private async Task LinkAsync()
    {
        if (Id is null)
            return;

        var localDate = _model.ScheduledDate ?? DateTime.Today;
        var localTime = _model.ScheduledTime ?? TimeSpan.Zero;
        var scheduled = TenantTimeZone.ToUtcOffset(localDate.Date + localTime, _tenantTimeZone);

        var parameters = new DialogParameters<LinkFlightDialog>
        {
            { x => x.IsArrival, _isArrival },
            { x => x.MetadataTime, scheduled },
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true };
        var dialog = await DialogService.ShowAsync<LinkFlightDialog>(
            $"Link to {(_isArrival ? "Departure" : "Arrival")}", parameters, options);
        var result = await dialog.Result;
        if (result is null || result.Canceled || result.Data is not long otherId)
            return;

        try
        {
            if (_isArrival)
                await Flights.LinkArrivalAsync(Id.Value, otherId);
            else
                await Flights.LinkDepartureAsync(Id.Value, otherId);

            _linkedFlightId = otherId;
            Snackbar.Add("Flights linked.", Severity.Success);

            if (OnLinkChanged.HasDelegate)
                await OnLinkChanged.InvokeAsync();
        }
        catch (ApiException ex)
        {
            Snackbar.Add($"Failed to link flight: {ex.Message}", Severity.Error);
        }
        catch (Exception)
        {
            Snackbar.Add("Could not reach the API while linking the flight.", Severity.Error);
        }
    }

    private async Task NavigateToLinkedAsync()
    {
        if (_linkedFlightId is not { } linkedId || !OnNavigateToLinked.HasDelegate)
            return;

        var linkedDirection = _isArrival ? "departure" : "arrival";
        await OnNavigateToLinked.InvokeAsync((linkedDirection, linkedId));
    }

    private static string? Blank(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;

    // Entering a time with no date yet is ambiguous, so default it to the flight's own scheduled
    // date (STA/STD) rather than leaving it blank — the estimate is almost always the same day.
    private void OnEtaTimeChanged(TimeSpan? value)
    {
        _model.EtaTime = value;
        _model.EtaDate ??= _model.ScheduledDate;
    }

    private void OnEtdTimeChanged(TimeSpan? value)
    {
        _model.EtdTime = value;
        _model.EtdDate ??= _model.ScheduledDate;
    }

    private (DateTime? Date, TimeSpan? Time) ToLocalPair(DateTimeOffset? utc)
    {
        if (utc is not { } value)
            return (null, null);

        var local = TenantTimeZone.ToLocal(value, _tenantTimeZone);
        return (local.Date, local.TimeOfDay);
    }

    // Requires both date and time — a date with no time was silently defaulting to midnight local,
    // which for a real timestamp (ETA/ATA/on-block/...) is materially wrong, not a reasonable default.
    private DateTimeOffset? ToUtcOffsetOrNull(DateTime? date, TimeSpan? time)
    {
        if (date is not { } d || time is not { } t)
            return null;

        return TenantTimeZone.ToUtcOffset(d.Date + t, _tenantTimeZone);
    }

    private string CountryTypeLabel(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return "—";

        var match = _countryTypes.FirstOrDefault(c => string.Equals(c.Code, code, StringComparison.OrdinalIgnoreCase));
        return match?.Label ?? code;
    }

    private Color StatusColor(string status) => FlightStatusColors.Resolve(_statusColors, status.ToUpperInvariant());
}
