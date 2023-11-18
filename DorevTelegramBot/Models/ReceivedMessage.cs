namespace DorevTelegramBot.Models;

public class ReceivedMessage
{
    public string Text { get; private set; }
    public DateTime Datetime { get; private set; }
    public long UserId { get; private set; }
    public string? Username { get; private set; }
    
    public ReceivedMessage(string text, DateTime datetime, long userId,
        string? username)
    {
        Text = text;
        Datetime = datetime;
        UserId = userId;
        Username = username;
    }

    public bool IsCommand()
    {
        return Text.ToLower() switch {
            "/start" => true,
            "/begin" => true,
            "/anywhere" => true,
            "/end" => true,
            "/feedback" => true,
            _ => false
        };
    }
}