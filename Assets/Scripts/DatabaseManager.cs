// Import necessary libraries
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Mono.Data.Sqlite;
using System.Data;
using System;

// DatabaseManager class responsible for managing database operations
public class DatabaseManager : MonoBehaviour
{
    // Text fields to display data from the database
    public TMP_Text levelReached_pro;
    public TMP_Text timePlayed_pro;
    public TMP_Text deathCount_pro;
    public TMP_Text killCount_pro;

    // Connection string for the database
    private string connectionString = "URI=file://" + Application.streamingAssetsPath + "/test.db";
    private IDbConnection dbConnection;

    // Start is called before the first frame update
    void Start()
    {
        // Open a connection to the database and read data
        dbConnection = (IDbConnection)new SqliteConnection(connectionString);
        dbConnection.Open();
        ReadData();
    }

    // Read data from the database and update the UI text fields
    public void ReadData()
    {
        // Variables to store the retrieved data
        int levelReached = 0;
        float timePlayed = 0f;
        int deathCount = 0;
        int killCount = 0;

        // Create a database command
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "SELECT * FROM Playthrough";
        IDataReader dataReader = dbCommand.ExecuteReader();

        // Read the data row by row
        while (dataReader.Read())
        {
            int lastLevel = dataReader.GetInt32(1);
            levelReached = lastLevel;
            levelReached_pro.text = levelReached.ToString();

            float time = dataReader.GetFloat(3);
            timePlayed = time;
            timePlayed_pro.text = timePlayed.ToString();

            int death = dataReader.GetInt32(4);
            deathCount = death;
            deathCount_pro.text = deathCount.ToString();

            int enemies = dataReader.GetInt32(5);
            killCount = enemies;
            killCount_pro.text = killCount.ToString();
        }

        // Close the data reader, dispose the command, and close the connection
        dataReader.Close();
        dbCommand.Dispose();
        dbConnection.Close();
    }

    // Insert data into the database
    public void SendData()
    {
        // Sample data to be inserted
        int gameId = 1;
        string level = "2";
        string health = "3";
        int time = 15;
        int death = 2;
        int globalId = 1;

        // SQL query to insert data
        string insertQuery = "INSERT INTO Playthrough (Id_Playthrough, lastLevel, Current_health, Play_time, Death_count, Id_global_statics ) VALUES (@Id_Playthrough, @lastLevel, @Current_health, @Play_time, @Death_count, @Id_global_statics)";

        // Create a SQLite command and set the parameters
        SqliteCommand insertCommand = new SqliteCommand(insertQuery, (SqliteConnection)dbConnection);
        insertCommand.Parameters.AddWithValue("@Id_Playthrough", gameId);
        insertCommand.Parameters.AddWithValue("@lastLevel", level);
        insertCommand.Parameters.AddWithValue("@Current_health", health);
        insertCommand.Parameters.AddWithValue("@Play_time", time);
        insertCommand.Parameters.AddWithValue("@Death_count", death);
        insertCommand.Parameters.AddWithValue("@Id_global_statics", globalId);

        // Execute the insert command
        insertCommand.ExecuteNonQuery();
    }
}