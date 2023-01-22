using Telegram.Bot.Types;

namespace WanderlustTgBot.Web.Abstractions;

public interface IReplyService
{
    Task ReplyWithUsageMessage(Message message);
    Task RequestSearchTypeAsync(Message message, string departureCode);
}