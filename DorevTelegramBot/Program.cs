using DorevTelegramBot;

string connectionString = "Data Source=./dorev.db;Mode=ReadOnly";
string token = "token";

using CancellationTokenSource cts = new();
Bot bot = new Bot(connectionString, token, cts.Token);

await bot.Run();
while(true) {
    string? o = Console.ReadLine();
    if (o?.ToLower() == "exit" || o?.ToLower() == "quit")
    {
        cts.Cancel();
        return;
    }
}
