namespace DorevConsoleApp;

public class Arguments
{
    public string Option { get; set; }
    public string Word { get; set; }

    public Arguments()
    {
    }

    public Arguments(string word, string option = "-b")
    {
        Word = word;
        Option = option;
    }
}
