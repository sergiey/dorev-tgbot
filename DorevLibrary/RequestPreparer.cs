using System.Text;

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
}
