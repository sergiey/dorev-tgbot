using System.Globalization;
using DorevLibrary;
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
    private readonly Vocabulary _vocab;
    private readonly CancellationToken _cancelToken;
    private readonly Dictionary<long, Options> _option = new ();
    private readonly ICatalog _catalog =
        new Catalog("Bot", "./Locale", new CultureInfo("ru-RU"));

    public Bot(string connectionString, string token,
        CancellationToken cancelToken)
    {
        Token = token;
        _cancelToken = cancelToken;
        _vocab = new Vocabulary(connectionString);
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

        Console.WriteLine($"Start listening for @{me.Username}");
    }

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if(update.Message is not { } message)
            return;
        if(message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;

        switch (message.Text.ToLower())
        {
            case "/start": {
                await ExecuteStartCase(botClient, message, chatId);
                return;
            }
            case "/begin": {
                await ExecuteBeginCase(botClient, message, chatId);
                return;
            }
            case "/anywhere": {
                await ExecuteAnywhereCase(botClient, message, chatId);
                return;
            }
            case "/end": {
                await ExecuteEndCase(botClient, message, chatId);
                return;
            }
        }

        Console.WriteLine(
            $"Received a '{messageText}' message in chat {chatId}.");

        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: FindWordInVocabulary(messageText, chatId),
            cancellationToken: cancellationToken);
    }

    private async Task ExecuteEndCase(ITelegramBotClient botClient,
        Message message, long chatId)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            _catalog.GetString("Search at the end of a word is enabled"));
        _option[chatId] = Options.MatchEnd;
    }

    private async Task ExecuteAnywhereCase(ITelegramBotClient botClient,
        Message message, long chatId)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            _catalog.GetString("Search anywhere in the word is enabled"));
        _option[chatId] = Options.MatchAnywhere;
    }

    private async Task ExecuteBeginCase(ITelegramBotClient botClient,
        Message message, long chatId)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            _catalog.GetString(
                "Search at the beginning of the word is enabled"));
        _option[chatId] = Options.MatchBegin;
    }

    private async Task ExecuteStartCase(ITelegramBotClient botClient,
        Message message, long chatId)
    {
        await botClient.SendTextMessageAsync(message.Chat, 
            _catalog.GetString("Submit a word to search"));
        _option[chatId] = Options.MatchBegin;
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

    private string FindWordInVocabulary(string messageText, long chatId)
    {
        _option.TryAdd(chatId, Options.MatchBegin);

        var result = _vocab.Translate(messageText, _option[chatId]);

        if(result != null)
            return result;

        result = _vocab.GetPresumableSpelling(messageText);

        if (result == messageText)
            return _catalog.GetString(
                "Word not found. Most likely, that’s how it’s written");

        if(result != null)
            return _catalog.GetString(
                "Word not found. Supposedly spelled like this: ") + result;

        return _catalog.GetString("Word not found");
    }
}
