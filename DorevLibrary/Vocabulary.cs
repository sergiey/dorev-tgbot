using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

namespace DorevLibrary;

public enum Options
{
    MatchBegin,
    MatchEnd,
    MatchAnywhere
}

public class Vocabulary
{
    private readonly string _connectionString;

    public Vocabulary(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string? Translate(string origin, Options option = Options.MatchBegin)
    {
        const string sqlExpression = @"
            SELECT Modern m, Trad t
            FROM Dictionary
            WHERE m REGEXP @word";
        
        if (origin == null)
            throw new ArgumentNullException();

        var preparedOrigin = GetRegexpPreparedString(origin, option);

        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.CreateFunction(
                "regexp",
                (string pattern, string input) =>
                    Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase));

            connection.Open();

            SqliteCommand command = new SqliteCommand(
                sqlExpression, connection);

            command.Parameters.AddWithValue("@word", preparedOrigin);

            return ReadResultsFromDb(command);
        }
    }

    private static string? ReadResultsFromDb(SqliteCommand command)
    {
        var result = new StringBuilder();
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            int count = 0;
            if(reader.HasRows) {
                while(reader.Read() && count < 20) {
                    result.Append($"{reader.GetString(1)}\n");
                    count++;
                }
            }
            else return null;
        }
        return result.ToString();
    }

    private static string GetRegexpPreparedString(string origin, Options option)
    {
        return option switch {
            Options.MatchBegin =>
                RegexpPreparer.GetBeginStringMatchRegexp(origin),
            Options.MatchEnd =>
                RegexpPreparer.GetEndStringMatchRegexp(origin),
            Options.MatchAnywhere =>
                RegexpPreparer.GetAnywhereMatchRegexp(origin),
            _ => RegexpPreparer.GetBeginStringMatchRegexp(origin)
        };
    }
}
