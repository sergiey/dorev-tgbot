using DorevLibrary;

namespace DorevTelegramBot;

internal class OptionsAccessor
{
    private static readonly Dictionary<long, Options> Option = new ();

    public static Options GetOption(long id)
    {
        if(Option.TryGetValue(id, out var option))
            return option;

        Option[id] = Options.MatchBegin;
        
        return Option[id];
    }

    public static void SetOption(long id, Options val)
    {
        if(Option.ContainsKey(id)) {
            Option[id] = val;
            return;
        }

        Option.Add(id, val);
    }
}
