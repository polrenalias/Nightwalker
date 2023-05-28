// Import necessary libraries
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LevelManager class responsible for managing level-related data
public class LevelManager : MonoBehaviour
{
    // Variables to store level data
    public int deathCount = 0;
    public int killCount = 0;
    public int levelReached = 0;
    public int currentHealth = 0;
    public float playedTime = 0.0f;
    //public DatabaseManager dbManager;

    void Awake()
    {
        //dbManager.GetData();
    }

    void Update()
    {
        // Update the played time
        playedTime += Time.deltaTime;
    }

    // Increase the death count
    public void IncreaseDeaths()
    {
        deathCount++;
    }

    // Get the death count
    public int GetDeathCount()
    {
        return deathCount;
    }

    // Increase the kill count
    public void IncreaseKills()
    {
        killCount++;
    }

    // Get the kill count
    public int GetKillCount()
    {
        return killCount;
    }

    // Set the last reached level
    public void SetLastLevel(int level)
    {
        levelReached = level;
    }

    // Get the last reached level
    public int GetLastLevel()
    {
        return levelReached;
    }

    // Set the current health value
    public void SetCurrentHealth(int health)
    {
        currentHealth = health;
    }

    // Get the current health value
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    // Set the played time
    public void SetPlayedTime(float time)
    {
        playedTime = time;
    }

    // Get the played time
    public float GetPlayedTime()
    {
        return playedTime;
    }
}
