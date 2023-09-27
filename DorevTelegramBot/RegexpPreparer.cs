using System.Text;

namespace DorevTelegramBot;

public class RegexpPreparer
{
    public static string NeutralizeIo(string origin)
    {
        var result = new StringBuilder();
        foreach(var c in origin) {
            if(c == 'е')
                result.Append("[её]");
            else
                result.Append(c);
        }
        return result.ToString();
    }
}
