using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

namespace DorevTelegramBot
{
    class SqliteDbRepository : IRepository
    {
        private readonly string _connectionString;

        public SqliteDbRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string Translate(string origin)
        {
            StringBuilder result = new StringBuilder();
            if (origin == null)
                throw new ArgumentNullException();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.CreateFunction(
                    "regexp",
                    (string pattern, string input)
                        => Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase));

                string sqlExpression = @$"
                    SELECT Modern m, Trad t
                    FROM Dictionary
                    WHERE m REGEXP @word";

                connection.Open();
                SqliteCommand command = new SqliteCommand(
                    sqlExpression, connection);
                    command.Parameters.AddWithValue(
                        "@word",
                        RegexpPreparer.NeutralizeIo(origin));

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    int count = 0;
                    if(reader.HasRows) {
                        while(reader.Read() && count < 20) {
                            result.Append($"{reader.GetString(1)}\n");
                            count++;
                        }
                    }
                    else return "Ничего не найдено";
                }
            }
            // return "h";
            return result.ToString();
        }
    }
}
