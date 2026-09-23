namespace RiverLine.Api.Models.Dtos.NotificationsDto;

public sealed record EmailMessageDto(
    string To,
    string RecipientName,
    string TemplateKey,
    IReadOnlyDictionary<string, JsonElement> Data);