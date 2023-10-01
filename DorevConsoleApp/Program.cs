using DorevTelegramBot;
string connectionString = "Data Source=absolute/path/dorev.db";
SqliteDbRepository rep = new SqliteDbRepository(connectionString);

try
{
    if (args.Length < 1) {
        Console.WriteLine("Какое слово вы ищите?");
        return;
    }
    Console.WriteLine(rep.Translate(args[0]));
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
