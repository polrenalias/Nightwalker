// Import necessary libraries
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// LevelManager class responsible for managing level-related data
/// </summary>
public class LevelManager : MonoBehaviour
{
    // Variables to store level data
    private int deathCount = 0;
    private int killCount = 0;
    private int lastLevel = 0;
    private int currentHealth = 0;
    private float playTime = 0f;

    public DatabaseManager manager;

    /// <summary>
    /// Initialize necessary components
    /// </summary>
    void Awake()
    {
        SetInitialValues();
    }

    /// <summary>
    /// Takes values from the DbManager and sets them on Awake
    /// </summary>
    private void SetInitialValues()
    {
        List<int> list = manager.GetDataList();

        if (list != null) {
            lastLevel = list[0];
            currentHealth = list[1];
            playTime = (float)list[2];
            deathCount = list[3];
            killCount = list[4];
        }
    }

    /// <summary>
    /// Takes values from the current game and sends them to the DbManager
    /// </summary>
    private void SetFinalValues()
    {
        List<int> list = new List<int>();

        list.Add(lastLevel);
        list.Add(currentHealth);
        list.Add(Convert.ToInt32(playTime));
        list.Add(deathCount);
        list.Add(killCount);

        manager.SetDataList(list);
    }

    /// <summary>
    /// Increase the death count
    /// </summary>
    public void IncreaseDeaths()
    {
        deathCount++;
    }

    /// <summary>
    /// Get the death count
    /// </summary>
    /// <returns></returns>
    public int GetDeathCount()
    {
        return deathCount;
    }

    /// <summary>
    /// Increase the kill count
    /// </summary>
    public void IncreaseKills()
    {
        killCount++;
    }

    /// <summary>
    /// Get the kill count
    /// </summary>
    /// <returns></returns>
    public int GetKillCount()
    {
        return killCount;
    }

    /// <summary>
    /// Set the last reached level
    /// </summary>
    /// <param name="level"></param>
    public void SetLastLevel(int level)
    {
        lastLevel = level;
    }

    /// <summary>
    /// Get the last reached level
    /// </summary>
    /// <returns></returns>
    public int GetLastLevel()
    {
        return lastLevel;
    }

    /// <summary>
    /// Set the current health value
    /// </summary>
    /// <param name="health"></param>
    public void SetCurrentHealth(int health)
    {
        currentHealth = health;
    }

    /// <summary>
    /// Get the current health value
    /// </summary>
    /// <returns></returns>
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Set the played time
    /// </summary>
    /// <param name="time"></param>
    public void SetPlayTime(float time)
    {
        playTime = time;
    }

    /// <summary>
    /// Get the played time
    /// </summary>
    public float GetPlayTime()
    {
        return playTime;
    }

    /// <summary>
    /// Pause handler
    /// </summary>
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus == true) {
            SetFinalValues();
        }
    }

    /// <summary>
    /// Exit handler
    /// </summary>
    void OnApplicationQuit()
    {
        SetFinalValues();
    }
}
