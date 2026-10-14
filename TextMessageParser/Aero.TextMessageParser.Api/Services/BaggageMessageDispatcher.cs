using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Parsers;
using Aero.TextMessageParser.Api.Parsers.Messages;
using Aero.TextMessageParser.Api.Persistence;

namespace Aero.TextMessageParser.Api.Services;

public sealed class BaggageMessageDispatcher<TDto>(
    MessageBase<TDto> parser,
    TextMessageParserDbContext textMessageParserDbContext,
    IHttpClientFactory httpClientFactory) : IMessageParserDispatcher
{
    public async Task<bool> DispatchAsync(TextMessage message, CancellationToken cancellationToken)
    {
        var dto = parser.ProcessMessage(message, textMessageParserDbContext);
        if (dto is not null)
        {
            var client = httpClientFactory.CreateClient("scannerapi");
            var res = await client.PostAsJsonAsync($"api/baggagemessages", dto, cancellationToken);
            return res.IsSuccessStatusCode;
        }
        return false;
    }
}
