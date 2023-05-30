// Import necessary libraries
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;
using System.Data;
using System;

/// <summary>
/// DatabaseManager class responsible for managing database operations
/// </summary>
public class DatabaseManager : MonoBehaviour
{
    // Game-related data list
    private List<int> dataList = new List<int>();
    // Connection for the database
    private IDbConnection dbConnection;

    /// <summary>
    /// Creates and opens a connection to the database
    /// </summary>
    void Awake()
    {
        string connectionString = GetConnectionString();
        dbConnection = (IDbConnection)new SqliteConnection(connectionString);
        ReadData();
    }

    /// <summary>
    /// Handles the database acquisition in the different platforms
    /// </summary>
    private string GetConnectionString()
    {
        string connectionString = string.Empty;
        #if UNITY_EDITOR
            // Editor platform (Windows)
            connectionString = "URI=file:" + Path.Combine(Application.streamingAssetsPath, "data.db");
        #elif UNITY_ANDROID
            // Android platform
            string dbFileName = "data.db";
            string dbPath = Path.Combine(Application.persistentDataPath, dbFileName);

            if (!File.Exists(dbPath))
            {
                // Check if the file exists in the streaming assets
                string streamingAssetsDBPath = Path.Combine(Application.streamingAssetsPath, dbFileName);

                if (streamingAssetsDBPath.Contains("://"))
                {
                    // If the file is in the StreamingAssets folder inside an APK, use UnityWebRequest to copy it
                    var www = UnityWebRequest.Get(streamingAssetsDBPath);
                    www.SendWebRequest();
                    while (!www.isDone) { } // Wait for file to be copied
                    File.WriteAllBytes(dbPath, www.downloadHandler.data);
                }
                else
                {
                    // If the file is in the StreamingAssets folder outside an APK (Unity Editor), use File.Copy to copy it
                    File.Copy(streamingAssetsDBPath, dbPath);
                }
            }

            connectionString = "URI=file:" + dbPath;
        #endif
        return connectionString;
    }

    /// <summary>
    /// Retrieve data from the database
    /// </summary>
    public void ReadData()
    {
        dbConnection.Open();

        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "SELECT * FROM PlaythroughStatistics WHERE id=1";
        IDataReader dataReader = dbCommand.ExecuteReader();

        while (dataReader.Read())
        {
            dataList.Add(dataReader.GetInt32(1));
            dataList.Add(dataReader.GetInt32(2));
            dataList.Add(dataReader.GetInt32(3));
            dataList.Add(dataReader.GetInt32(4));
            dataList.Add(dataReader.GetInt32(5));
        }

        dataReader.Close();
        dbCommand.Dispose();
        dbConnection.Close();
        dbConnection.Dispose();
    }

    /// <summary>
    /// Method for data accessibiltiy
    /// </summary>
    public List<int> GetDataList() 
    {
        return dataList;
    }

    /// <summary>
    /// Method for data accessibiltiy
    /// </summary>
    public void SetDataList(List<int> list) 
    {
        dataList = list;
        WriteData();
    }

    /// <summary>
    /// Add or modify current game data into the database
    /// </summary>
    public void WriteData()
    {
        dbConnection.Open();
        
        // Check if a record with the specified ID exists
        IDbCommand checkCommand = dbConnection.CreateCommand();
        checkCommand.CommandText = "SELECT COUNT(*) FROM PlaythroughStatistics WHERE id = 1";
        int recordCount = Convert.ToInt32(checkCommand.ExecuteScalar());
        checkCommand.Dispose();

        if (recordCount > 0)
        {
            // SQL query to update data
            string updateQuery = "UPDATE PlaythroughStatistics " +
                                "SET Last_Level = @Last_Level, " +
                                "Current_Health = @Current_Health, " +
                                "Play_Time = @Play_Time, " +
                                "Death_Count = @Death_Count, " +
                                "Kill_Count = @Kill_Count " +
                                "WHERE id = 1";

            // Create a SQLite command and set the parameters
            using (SqliteCommand updateCommand = new SqliteCommand(updateQuery, (SqliteConnection)dbConnection))
            {
                updateCommand.Parameters.AddWithValue("@Last_Level", dataList[0]);
                updateCommand.Parameters.AddWithValue("@Current_Health", dataList[1]);
                updateCommand.Parameters.AddWithValue("@Play_Time", dataList[2]);
                updateCommand.Parameters.AddWithValue("@Death_Count", dataList[3]);
                updateCommand.Parameters.AddWithValue("@Kill_Count", dataList[4]);

                // Execute the insert command
                updateCommand.ExecuteNonQuery();
            }
        }
        else
        {
            string insertQuery = "INSERT INTO PlaythroughStatistics (Last_Level, Current_Health, Play_Time, Death_Count, Kill_Count) VALUES (@Last_Level, @Current_Health, @Play_Time, @Death_Count, @Kill_Count)";

            // Create a SQLite command and set the parameters
            using (SqliteCommand insertCommand = new SqliteCommand(insertQuery, (SqliteConnection)dbConnection))
            {
                insertCommand.Parameters.AddWithValue("@Last_Level", dataList[0]);
                insertCommand.Parameters.AddWithValue("@Current_Health", dataList[1]);
                insertCommand.Parameters.AddWithValue("@Play_Time", dataList[2]);
                insertCommand.Parameters.AddWithValue("@Death_Count", dataList[3]);
                insertCommand.Parameters.AddWithValue("@Kill_Count", dataList[4]);

                // Execute the insert command
                insertCommand.ExecuteNonQuery();
            }
        }

        dbConnection.Close();
        dbConnection.Dispose();
    }
}