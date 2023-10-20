using DorevTelegramBot;
using Microsoft.Extensions.Configuration;

string connectionString = "Data Source=./dorev.db;Mode=ReadOnly";
IConfiguration config =
    new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string token = config["Token"]!;

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
