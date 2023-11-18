using System.Globalization;
using DorevLibrary;
using DorevTelegramBot.Models;
using GetText;
using Telegram.Bot;

namespace DorevTelegramBot;

public class CommandHandler
{
    private readonly ICatalog _catalog;
    private readonly SettingsAccessor _settings = new(); 

    public CommandHandler(ICatalog catalog)
    {
        _catalog = catalog;
    }
    
    public async Task Handle(ReceivedMessage message,
        ITelegramBotClient botClient)
    {
        var command = message.Text.ToLower();
        switch (command)
        {
            case "/start":
            case "/begin":
            case "/anywhere":
            case "/end": {
                await ExecuteStartOrOptionCase(message, command, botClient);
                return;
            }    
            case "/feedback": {
                await ExecuteFeedbackCase(message, botClient);
                return;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private async Task ExecuteStartOrOptionCase(ReceivedMessage message, 
        string command, ITelegramBotClient botClient)
    {
        var dt = message.Datetime.ToString(new CultureInfo("ru-RU"));
        var reply = command switch {
            "/start" => new[] {
                _catalog.GetString("Submit a word to search"),
                $"{dt} User '{message.Username}' ({message.UserId}) " +
                "has enabled search in the end"
            },
            "/begin" => new[] {
                _catalog.GetString(
                    "Search at the beginning of the word is enabled"),
                $"{dt} User '{message.Username}' ({message.UserId}) " +
                "has enabled search in the beginning"
            },
            "/anywhere" => new[] {
                _catalog.GetString("Search anywhere in the word is enabled"),
                $"{dt} User '{message.Username}' ({message.UserId}) " +
                "has enabled search anywhere"
            },
            "/end" => new[] {
                _catalog.GetString("Search at the end of a word is enabled"),
                $"{dt} User '{message.Username}' ({message.UserId}) " +
                "has enabled search in the end"
            },
            _ => new[] {
                _catalog.GetString("Unknown command"),
                "Unknown command"    
            } 
        };

        var option = command switch {
            "/start" => Options.MatchBegin,
            "/begin" => Options.MatchBegin,
            "/anywhere" => Options.MatchAnywhere,
            "/end" => Options.MatchEnd,
            _ => Options.MatchBegin
        };

        await botClient.SendTextMessageAsync(message.UserId, reply[0]);
        Console.WriteLine(reply[1]);
        _settings.SetOption(message.UserId, option);
    }

    private async Task ExecuteFeedbackCase(ReceivedMessage message,
        ITelegramBotClient botClient)
    {
        var dt = message.Datetime.ToString(new CultureInfo("ru-RU"));
        await botClient.SendTextMessageAsync(message.UserId,
            _catalog.GetString("Leave feedback or report a mistake"));
        Console.WriteLine($"{dt} User '{message.Username}' " +
                          $"({message.UserId}) leaves feedback");
        _settings.SetFeedback(message.UserId, true);
    }
}