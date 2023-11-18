using DorevLibrary;
using Microsoft.Data.Sqlite;

namespace DorevTelegramBot;

internal class SettingsAccessor
{
    private const string ConnectionString = 
        "Data Source=./Resources/settings.db";

    public Options GetOption(long id)
    {
        const string sqlExpression = @"
            SELECT Option
            FROM Settings
            WHERE User_Id = @id";
        
        using var connection = new SqliteConnection(ConnectionString);
        
        connection.Open();
        
        var command = new SqliteCommand();
        command.Connection = connection;
        command.CommandText = sqlExpression;
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        
        if(!reader.HasRows) {
            SetOption(id, Options.MatchBegin);
            return Options.MatchBegin;
        }

        reader.Read();
        
        return reader["Option"] switch {
            "MatchBegin" => Options.MatchBegin,
            "MatchAnywhere" => Options.MatchAnywhere,
            "MatchEnd" => Options.MatchEnd,
            _ => Options.MatchBegin
        };
    }

    public void SetOption(long id, Options option)
    {
        if(IsOptionSet(id)) {
            UpdateOption(id, option);
            return;
        }

        InsertOption(id, option);
    }

    private static void InsertOption(long id, Options option)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = new SqliteCommand();
        command.Connection = connection;
        command.CommandText = @"
            INSERT INTO Settings (User_Id, Option)
            VALUES (@id, @opt)";
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@opt", option.ToString());

        command.ExecuteNonQuery();
    }

    private static void UpdateOption(long id, Options option)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = new SqliteCommand();
        command.Connection = connection;
        command.CommandText = @"
            UPDATE Settings 
            SET Option = @opt
            WHERE User_Id = @id";
        command.Parameters.AddWithValue("@opt", option.ToString());
        command.Parameters.AddWithValue("@id", id);

        command.ExecuteNonQuery();
    }

    private static bool IsOptionSet(long id)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = new SqliteCommand();
        command.Connection = connection;
        command.CommandText = @"
            SELECT Option
            FROM Settings
            WHERE User_Id = @id";
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        
        return reader.HasRows;
    }

    public bool IsFeedback(long id)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        
        var command = new SqliteCommand();
        command.Connection = connection;
        command.CommandText = @"
            SELECT Feedback
            FROM Settings
            WHERE User_Id = @id";
        command.Parameters.AddWithValue("@id", id);
        
        using var reader = command.ExecuteReader();

        if(!reader.HasRows)
            return false;
        
        reader.Read();
        bool.TryParse(reader["Feedback"].ToString(), out var result);
        
        return result;
    }
    
    public void SetFeedback(long id, bool status)
    {
        var flag = status ? "true" : "false";

        if(IsFeedbackFlagSet(id)) {
            UpdateFeedbackFlag(id, flag);
            return;
        }
        
        InsertFeedbackFlag(id, flag);
    }

    private static void InsertFeedbackFlag(long id, string flag)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        
        var command = new SqliteCommand();
        command.Connection = connection;
        command.CommandText = @"
            INSERT INTO Settings (User_Id, Option, Feedback)
            VALUES (@id, @opt, @flag)";
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@opt", Options.MatchBegin.ToString());
        command.Parameters.AddWithValue("@flag", flag);
        
        command.ExecuteNonQuery();
    }

    private static void UpdateFeedbackFlag(long id, string flag)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        
        var command = new SqliteCommand();
        command.Connection = connection;
        command.CommandText = @"
            UPDATE Settings 
            SET Feedback = @flag
            WHERE User_Id = @id";
        command.Parameters.AddWithValue("@flag", flag);
        command.Parameters.AddWithValue("@id", id);
        
        command.ExecuteNonQuery();
    }

    private static bool IsFeedbackFlagSet(long id)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        
        var command = new SqliteCommand();
        command.Connection = connection;
        command.CommandText = @"
            SELECT Feedback
            FROM Settings
            WHERE User_Id = @id";
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();

        return reader.HasRows; 
    }
}