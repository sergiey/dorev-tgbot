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

public class SqliteDbRepository
{
    private readonly string _connectionString;

    public SqliteDbRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string Translate(string origin, Options option = Options.MatchBegin)
    {
        StringBuilder result = new StringBuilder();
        if (origin == null)
            throw new ArgumentNullException();

        string word = option switch {
            Options.MatchBegin => 
                RegexpPreparer.GetBeginStringMatchRegexp(origin),
            Options.MatchEnd => 
                RegexpPreparer.GetEndStringMatchRegexp(origin),
            Options.MatchAnywhere => 
                RegexpPreparer.GetAnywhereMatchRegexp(origin),
            _ => RegexpPreparer.GetBeginStringMatchRegexp(origin)
        };

        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.CreateFunction(
                "regexp",
                (string pattern, string input) =>
                    Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase));

            string sqlExpression = @$"
                    SELECT Modern m, Trad t
                    FROM Dictionary
                    WHERE m REGEXP @word";

            connection.Open();

            SqliteCommand command = new SqliteCommand(
                sqlExpression, connection);

            command.Parameters.AddWithValue("@word", word);

            using (SqliteDataReader reader = command.ExecuteReader())
            {
                int count = 0;
                if(reader.HasRows) {
                    while(reader.Read() && count < 20) {
                        result.Append($"{reader.GetString(1)}\n");
                        count++;
                    }
                }
                else return "Nothing found";
            }
        }
        return result.ToString();
    }
}