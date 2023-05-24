using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int deathCount = 0;
    public int killCount = 0;
    public int levelReached = 0;
    public float playedTime = 0.0f;
    //public DatabaseManager dbManager;

    void Awake()
    {
        //dbManager.GetData();
    }

    void Update()
    {
        playedTime += Time.deltaTime;
        //timeDisplay.text = Mathf.RoundToInt(playedTime).ToString();
    }
    
    public void IncreaseDeaths()
    {
        deathCount++;
    }

    public int GetDeathCount()
    {
       return deathCount;
    }

    public void IncreaseKills()
    {
        killCount++;
    }

    public int GetKillCount()
    {
       return killCount;
    }

    public void SetLastLevel(int level)
    {
        levelReached = level;
    }

    public int GetLastLevel()
    {
       return levelReached;
    }

    public void SetPlayedTime(float time)
    {
        playedTime = time;
    }

    public float GetPlayedTime()
    {
        return playedTime;
    }
}
