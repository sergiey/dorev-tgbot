using System.Text;

namespace DorevTelegramBot;

public class CsvDataHelper
{
    public static async Task AppendLine(params string[] list)
    {
        const string path = "./log.csv";

        var sb = new StringBuilder();
        for(int i = 0; i < list.Length; i++) {
            sb.Append(list[i]);
            if(i == list.Length - 1)
                break;
            sb.Append(';');
        }

        sb.Append('\n');

        await using var fs = File.Open(path, FileMode.Append);
        var data = new UTF8Encoding(true).GetBytes(sb.ToString());
        await fs.WriteAsync(data);
    }
}
