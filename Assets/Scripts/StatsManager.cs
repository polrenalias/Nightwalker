using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;

/// <summary>
/// StatsManager class responsible for handling the statistics screen
/// </summary>
public class StatsManager : MonoBehaviour
{
    // Text fields to display data from the database
    public TMP_Text levelReached_pro;
    public TMP_Text timePlayed_pro;
    public TMP_Text deathCount_pro;
    public TMP_Text killCount_pro;
    
    public LevelManager manager;

    /// <summary>
    /// Display the values obtained from LevelManager on the stat screen
    /// </summary>
    void Awake()
    {
        levelReached_pro.text = manager.GetLastLevel().ToString();
        killCount_pro.text = manager.GetKillCount().ToString();
        deathCount_pro.text = manager.GetDeathCount().ToString();
        int time = Convert.ToInt32(manager.GetPlayTime() / 60);
        if (time < 1)  time = 0;
        timePlayed_pro.text = time.ToString()+" m";
    }
}
