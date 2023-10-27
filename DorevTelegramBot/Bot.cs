using DorevLibrary;
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

    async Task HandleUpdateAsync(
        ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if(update.Message is not { } message)
            return;
        if(message.Text is not { } messageText)
            return;
        if(message.Text.ToLower() == "/start"){
            await botClient.SendTextMessageAsync(
                message.Chat,
                "Отправьте искомое слово");
            return;
        }

        var chatId = message.Chat.Id;
        Console.WriteLine(
            $"Received a '{messageText}' message in chat {chatId}.");

        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: FindWordInVocabulary(messageText),
            cancellationToken: cancellationToken);
    }

    Task HandlePollingErrorAsync(
        ITelegramBotClient botClient,
        Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n" +
                    $"{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }

    private string FindWordInVocabulary(string messageText)
    {
        var result = _vocab.Translate(messageText);

        if(result != null)
            return result;

        result = _vocab.GetPresumableSpelling(messageText);

        if (result == messageText)
            return "Слово не найдено. Скорѣе всего, такъ и пишется";

        if(result != null)
            return "Слово не найдено. Предположеніе: " + result;

        return "Слово не найдено";
    }
}
