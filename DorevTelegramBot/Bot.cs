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
    private readonly Cases _cases = new (Catalog);
    private static readonly ICatalog Catalog =
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

        var chatId = message.Chat.Id;

        switch (message.Text.ToLower())
        {
            case "/start": {
                await _cases.ExecuteStartCase(botClient, message, chatId);
                return;
            }
            case "/begin": {
                await _cases.ExecuteBeginCase(botClient, message, chatId);
                return;
            }
            case "/anywhere": {
                await _cases.ExecuteAnywhereCase(botClient, message, chatId);
                return;
            }
            case "/end": {
                await _cases.ExecuteEndCase(botClient, message, chatId);
                return;
            }
        }

        var dt = DateTime.Now.ToString(new CultureInfo("ru-RU"));
        Console.WriteLine($"{dt} Received '{messageText}'" +
                          $" from '{message.From?.Username}' ({chatId})");

        await CsvDataHelper.AppendLine(dt, message.From?.Username!,
            chatId.ToString(), messageText,
            OptionsAccessor.GetOption(chatId).ToString());

        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: FindWordInVocabulary(messageText, chatId),
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

    private string FindWordInVocabulary(string messageText, long chatId)
    {
        var result = _vocab.Translate(messageText,
            OptionsAccessor.GetOption(chatId));

        if(result != null)
            return result;

        result = _vocab.GetPresumableSpelling(messageText);

        if (result == messageText)
            return Catalog.GetString(
                "Word not found. Most likely, that’s how it’s written");

        if(result != null)
            return Catalog.GetString(
                "Word not found. Supposedly spelled like this: ") + result;

        return Catalog.GetString("Word not found");
    }
}
