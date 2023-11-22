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
        const int minShrinkWordLength = 7;
        const string sqlExpression = @"
            SELECT Modern, Trad
            FROM Dictionary
            WHERE Modern REGEXP @word";

        if (origin == null)
            throw new ArgumentNullException();

        var normalizedWord = RequestPreparer.GetNormalizedWord(origin);
        var regexpString = GetRegexpPreparedString(normalizedWord, option);

        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.CreateFunction(
                "regexp",
                (string pattern, string input) =>
                    Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase));

            connection.Open();

            SqliteCommand command = new SqliteCommand(
                sqlExpression, connection);

            command.Parameters.AddWithValue("@word", regexpString);
            var result = ReadResultsFromDb(command);

            // If the word is not found, then shrink them & repeat the search
            if(result == null && normalizedWord.Length >= minShrinkWordLength)
            {
                normalizedWord = RequestPreparer.ShrinkWord(normalizedWord);
                regexpString = GetRegexpPreparedString(normalizedWord, option);
                
                command = new SqliteCommand(sqlExpression, connection);
                command.Parameters.AddWithValue("@word", regexpString);
                
                return ReadResultsFromDb(command);
            }

            return result;
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
        var normalizedWord = RequestPreparer.GetNormalizedWord(origin);

        return option switch {
            Options.MatchBegin =>
                RequestPreparer.GetBeginStringMatchRegexp(normalizedWord),
            Options.MatchEnd =>
                RequestPreparer.GetEndStringMatchRegexp(normalizedWord),
            Options.MatchAnywhere =>
                RequestPreparer.GetAnywhereMatchRegexp(normalizedWord),
            _ => RequestPreparer.GetBeginStringMatchRegexp(normalizedWord)
        };
    }

    public string GetPresumableSpelling(string origin)
    {
        if(origin == null)
            throw new ArgumentNullException();

        var regexp = new Regex(@"и(?=[аеёийоуѣыэюяѵ])");
        var result = regexp.Replace(origin.ToLower(), "і");

        regexp = new Regex(@"(с)(?<=\b(бес|черес|чрес))");
        result = regexp.Replace(result, "з");

        regexp = new Regex(@"(?<=\b(ра|во|ро|и))с(?=с)");
        result = regexp.Replace(result, "з");

        regexp = new Regex(@"([цкнгшбзхщфвпрлджчсмтѳ]\b)");
        return regexp.Replace(result, "$1ъ");
    }
}
