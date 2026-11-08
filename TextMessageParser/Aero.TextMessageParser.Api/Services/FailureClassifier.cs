using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Aero.TextMessageParser.Api.Services;

public static class FailureClassifier
{
    public static bool IsTransient(Exception exception) => exception switch
    {
        TimeoutException => true,
        IOException => true,
        HttpRequestException => true,
        DbException => true,
        DbUpdateException => true,
        _ => false
    };
}
