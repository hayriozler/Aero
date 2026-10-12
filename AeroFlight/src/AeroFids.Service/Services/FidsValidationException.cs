namespace AeroFids.Service.Services;

public sealed class FidsValidationException(string message) : Exception(message);
