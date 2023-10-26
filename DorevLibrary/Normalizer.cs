using System.Text.RegularExpressions;

namespace DorevLibrary;

public class Normalizer
{
    public static string GetNormalizedWord(string origin)
    {
        var str = origin.ToLower();
        str = str.Replace('ѣ', 'е');
        str = str.Replace('і', 'и');
        str = str.Replace('i', 'и');
        var regexp = new Regex(@"ъ\b");
        return regexp.Replace(str, string.Empty);
    }
}