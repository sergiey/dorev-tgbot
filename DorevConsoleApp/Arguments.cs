using DorevLibrary;

namespace DorevConsoleApp;

public class Arguments
{
    public Options Option { get; set; }
    public string? Word { get; set; }

    public Arguments()
    {
    }

    public Arguments(string word, Options option = Options.MatchBegin)
    {
        Word = word;
        Option = option;
    }
}
