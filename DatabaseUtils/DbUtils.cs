using System.Text;
using MySql.Data.MySqlClient;
using NureBotSchedule.Services;
using Tomlyn;

namespace NureBotSchedule.DatabaseUtils;

public class DbUtils
{
    public static MySqlConnection GetDBConnection()
    {
        string host;
        string database;
        string username;
        string password;
        
        using (FileStream fstream = File.OpenRead("config-bot.toml"))
        {
            byte[] buffer = new byte[fstream.Length];
            fstream.Read(buffer, 0, buffer.Length);
            string textFromFile = Encoding.Default.GetString(buffer);

            var model = Toml.ToModel(textFromFile);
            host = (string) model["addressDatabase"]!;
            database = (string) model["nameDatabase"]!;
            username = (string) model["nameUserDatabase"]!;
            password = (string) model["passwordUserDatabase"]!;
        }

        
        // Connection String.
        String connString = $"Server={host};Database={database};User Id={username};password={password}";

        MySqlConnection conn = new MySqlConnection(connString);

        return conn;
    }
    
    public static void CreateTableOrNo()
    {
        var conn = GetDBConnection();
        
        conn.Open();
        try
        {
            string query = "CREATE TABLE `chats` (" +
                           "`id` BIGINT NULL DEFAULT NULL ," +
                           "`group` INT NULL DEFAULT NULL," +
                           "`group_id` BIGINT NULL DEFAULT NULL " +
                           ") ENGINE = InnoDB;";
            MySqlCommand command = new MySqlCommand(query, conn);
            command.ExecuteNonQuery();
        }
        catch (MySqlException e)
        { }
        conn.Close();
    }
    
    public static Group? GetGroup(long id)
    {
        string sql = $"SELECT `group`,`group_id` FROM `chats` WHERE id = {id};";
        
        var conn = GetDBConnection();
        
        conn.Open();
        using var cmd = new MySqlCommand(sql, conn);
        using MySqlDataReader rdr = cmd.ExecuteReader();
        Group group = new Group();
        if (rdr.HasRows)
        {
            if (rdr.Read())
            {
                group.GroupNumber = rdr.GetInt32(0);
                group.GroupId = rdr.GetInt64(1);
                rdr.Close();
            }
        }
        else
        {
            group = null;
        }
        
        conn.Close();
        return group;
    }
    
    public static void InsertGroup(long id, int group, long group_id)
    {
        var conn = GetDBConnection();

        conn.Open();
        var sql = $"INSERT INTO chats VALUES({id}, {group}, {group_id});";
        using var command = new MySqlCommand(sql, conn);
        command.ExecuteNonQuery();
        conn.Close();
    }
    
    public static bool checkGroup(long id)
    {
        var conn = GetDBConnection();
        conn.Open();
        string query = "SELECT * FROM chats WHERE id = @number";
        using (MySqlCommand command = new MySqlCommand(query, conn)) {
            command.Parameters.AddWithValue("@number", id);
            using (MySqlDataReader reader = command.ExecuteReader()) {
                if (reader.HasRows) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    }

}