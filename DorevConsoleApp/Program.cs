using DorevConsoleApp;
using DorevLibrary;

const string usageText = """
Usage: dorev [option] [word]

Options:
    -b    search in the beginning of a word (default)
    -e    search in the end of a word
    -a    search anywhere in a word
""";

var dbPath = Path.GetDirectoryName(
                 System.Reflection.Assembly.GetExecutingAssembly().Location) +
             "/dorev.db";
var connectionString = $"Data Source={dbPath}";
var vocab = new Vocabulary(connectionString);

try
{
    var arguments = ArgumentsParser.GetArguments(args);
    Console.WriteLine(vocab.Translate(arguments.Word!, arguments.Option));
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    Console.WriteLine(usageText);
}
