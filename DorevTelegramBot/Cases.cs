using System.Globalization;
using DorevLibrary;
using GetText;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DorevTelegramBot;

public class Cases
{
    private readonly ICatalog _catalog;

    public Cases(ICatalog catalog)
    {
        _catalog = catalog;
    }

    internal async Task ExecuteStartCase(ITelegramBotClient botClient,
        Message message, long chatId)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            _catalog.GetString("Submit a word to search"));
        OptionsAccessor.SetOption(chatId, Options.MatchBegin);

        var dt = DateTime.Now.ToString(new CultureInfo("ru-RU"));
        Console.WriteLine($"{dt} User '{message.From?.Username}' " +
            $"({chatId}) activated the bot");
    }

    internal async Task ExecuteBeginCase(ITelegramBotClient botClient,
        Message message, long chatId)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            _catalog.GetString(
                "Search at the beginning of the word is enabled"));
        OptionsAccessor.SetOption(chatId, Options.MatchBegin);

        var dt = DateTime.Now.ToString(new CultureInfo("ru-RU"));
        Console.WriteLine($"{dt} User '{message.From?.Username}' " +
            $"({chatId}) has enabled search in the beginning");
    }

    internal async Task ExecuteAnywhereCase(ITelegramBotClient botClient,
        Message message, long chatId)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            _catalog.GetString("Search anywhere in the word is enabled"));
        OptionsAccessor.SetOption(chatId, Options.MatchAnywhere);

        var dt = DateTime.Now.ToString(new CultureInfo("ru-RU"));
        Console.WriteLine($"{dt} User '{message.From?.Username}' " +
            $"({chatId}) has enabled search anywhere");
    }

    internal async Task ExecuteEndCase(ITelegramBotClient botClient,
        Message message, long chatId)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            _catalog.GetString("Search at the end of a word is enabled"));
        OptionsAccessor.SetOption(chatId, Options.MatchEnd);

        var dt = DateTime.Now.ToString(new CultureInfo("ru-RU"));
        Console.WriteLine($"{dt} User '{message.From?.Username}' " +
            $"({chatId}) has enabled search in the end");
    }
}
