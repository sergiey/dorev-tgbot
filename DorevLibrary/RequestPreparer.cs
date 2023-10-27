using System.Text;
using System.Text.RegularExpressions;

namespace DorevLibrary;

public class RequestPreparer
{
    private static StringBuilder NeutralizeIo(string origin)
    {
        var result = new StringBuilder();
        foreach(var c in origin) {
            if(c == 'е')
                result.Append("[её]");
            else
                result.Append(c);
        }
        return result;
    }

    public static string GetBeginStringMatchRegexp(string origin)
    {
        return NeutralizeIo(origin).Insert(0, "\\A").ToString();
    }

    public static string GetEndStringMatchRegexp(string origin)
    {
        return NeutralizeIo(origin).Append("\\z").ToString();
    }

    public static string GetAnywhereMatchRegexp(string origin)
    {
        return NeutralizeIo(origin).ToString();
    }
    
    public static string GetNormalizedWord(string origin)
    {
        var str = origin.ToLower();
        str = str.Replace('ѣ', 'е');
        str = str.Replace('і', 'и');
        str = str.Replace('i', 'и');
        var regexp = new Regex(@"ъ\b");
        return regexp.Replace(str, string.Empty);
    }

    public static string ShrinkWord()
    {
        throw new NotImplementedException();
    }
}
