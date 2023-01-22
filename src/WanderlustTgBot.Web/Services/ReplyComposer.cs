using WanderlustTgBot.Web.Abstractions;

namespace WanderlustTgBot.Web.Services;

public class ReplyComposer : IReplyComposer
{
    public string GetUsageMessage()
        => "Для поиска авиабилетов напишите боту город отправления.";
}