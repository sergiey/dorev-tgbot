﻿using DorevTelegramBot;
using Microsoft.Extensions.Configuration;

IConfiguration config =
    new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string token = config["Token"]!;

using CancellationTokenSource cts = new();
Bot bot = new Bot(token, cts.Token);

try
{
    await bot.Run();
    while (true)
    {
        string? o = Console.ReadLine();
        if (o?.ToLower() == "exit" || o?.ToLower() == "quit")
            return;
    }
}
catch (Exception e)
{
    Console.Error.WriteLine($"{DateTime.Now}\nException:\n{e.Message}");
    Console.Error.WriteLine($"Method:\n{e.TargetSite}");
    Console.Error.WriteLine($"Stack trace:\n{e.StackTrace}");
}
finally
{
    cts.Cancel();
}
