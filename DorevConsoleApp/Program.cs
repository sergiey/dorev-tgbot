using DorevTelegramBot;
string connectionString = "Data Source=absolute/path/dorev.db";
SqliteDbRepository rep = new SqliteDbRepository(connectionString);
const string usageText = """
Usage: dorev [option] [word]

Options:
    -b    search in the beginning of a word (default)
    -e    search in the end of a word
    -a    search anywhere in a word
""";

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
