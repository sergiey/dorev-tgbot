using System.Globalization;
using DorevLibrary;
using DorevTelegramBot.Models;
using GetText;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DorevTelegramBot;

public class Bot
{
    public string Token;
    private readonly Vocabulary _vocab = new();
    private readonly CancellationToken _cancelToken;
    private readonly CommandHandler _commandHandler = new (Catalog);
    private readonly SettingsAccessor _settings = new(); 
    private static readonly ICatalog Catalog =
        new Catalog("Bot", "./Locale", new CultureInfo("ru-RU"));

    public Bot(string token, CancellationToken cancelToken)
    {
        Token = token;
        _cancelToken = cancelToken;
    }

    public async Task Run()
    {
        TelegramBotClient botClient = new TelegramBotClient(Token);

        ReceiverOptions receiverOptions = new() {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: _cancelToken
        );

        var me = await botClient.GetMeAsync();

        Console.WriteLine(
            $"{DateTime.Now} Start listening for @{me.Username}");
    }

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if(update.Message is not { } message)
            return;
        if(message.Text is not { } messageText)
            return;

        var msg = new ReceivedMessage(message.Text, DateTime.Now, 
            message.Chat.Id, message.From?.Username);
        
        var dt = msg.Datetime.ToString(new CultureInfo("ru-RU"));
        Console.WriteLine($"{dt} Received '{msg.Text}'" +
                          $" from '{msg.Username}' ({msg.UserId})");

        if(msg.IsCommand()) {
            await _commandHandler.Handle(msg, botClient);
            return;
        }
        
        await botClient.SendTextMessageAsync(
            chatId: msg.UserId,
            text: HandleRequest(msg).Result,
            cancellationToken: cancellationToken);
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient,
        Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch {
            ApiRequestException apiRequestException =>
                $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n" +
                $"{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }

    private async Task<string> HandleRequest(ReceivedMessage message)
    {
        var dt = message.Datetime.ToString(new CultureInfo("ru-RU"));
        
        if (_settings.IsFeedback(message.UserId)) {
            await CsvDataHelper.AppendLine(dt, message.Username!, 
                message.UserId.ToString(), message.Text, "Feedback");
            _settings.SetFeedback(message.UserId, false);
            
            return Catalog.GetString("Your feedback has been received");
        }
        
        await CsvDataHelper.AppendLine(dt, message.Username!, 
            message.UserId.ToString(), message.Text, 
            _settings.GetOption(message.UserId).ToString());
        
        var result = _vocab.Translate(message.Text,
            _settings.GetOption(message.UserId));

        if(result != null)
            return result;

        result = _vocab.GetPresumableSpelling(message.Text);

        if (result == message.Text)
            return Catalog.GetString(
                "Word not found. Most likely, that’s how it’s written");

        if(result != null)
            return Catalog.GetString(
                "Word not found. Supposedly spelled like this: ") + result;

        return Catalog.GetString("Word not found");
    }
}
