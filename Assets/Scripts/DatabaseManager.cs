/*using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Mono.Data.Sqlite;

public class DatabaseManager : MonoBehaviour
{
    private string connectionString;

    void Start()
    {
        // Set the connection string for the SQLite database
        connectionString = "URI=file:" + Application.persistentDataPath + "/gamedata.db";

        // Create a new database file if it doesn't exist
        CreateDatabase();
    }

    void CreateDatabase()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS mytable (id INTEGER PRIMARY KEY, name TEXT)";
                command.ExecuteNonQuery();
            }
        }
    }
}*/