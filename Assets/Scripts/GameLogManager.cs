using System;
using System.IO;
using UnityEngine;

public class GameLogManager : MonoBehaviour
{
    private float stage1ClearTime = -1f;
    private float stage2ClearTime = -1f;
    private float stage3ClearTime = -1f;
    private string rank = "";
    private bool logEnabled = true;

    private float startTime;

    void Start()
    {
        startTime = Time.time;
        logEnabled = true;
    }

    public void RecordStageClear(int stageNumber, float remainingTime)
    {
        if (!logEnabled) return;

        switch (stageNumber)
        {
            case 1:
                stage1ClearTime = remainingTime;
                break;
            case 2:
                stage2ClearTime = remainingTime;
                break;
            case 3:
                stage3ClearTime = remainingTime;
                break;
        }
    }

    public void SetRank(string sabcRank)
    {
        if (!logEnabled) return;
        rank = sabcRank;
    }

    public void WriteLog()
    {
        if (!logEnabled) return;

        string date = DateTime.Now.ToString("yyyy/MM/dd");
        string time = DateTime.Now.ToString("HH:mm");

        string s1 = stage1ClearTime >= 0 ? stage1ClearTime.ToString("F2") : "";
        string s2 = stage2ClearTime >= 0 ? stage2ClearTime.ToString("F2") : "";
        string s3 = stage3ClearTime >= 0 ? stage3ClearTime.ToString("F2") : "";

        string line = string.Format("{0},{1},{2},{3},{4},{5}", date, time, s1, s2, s3, rank);

        string path = Path.Combine(Application.persistentDataPath, "game_log.csv");
        File.AppendAllText(path, line + "\n");

        Debug.Log("[GameLogManager] Log written: " + line);

        logEnabled = false; // ˆê“x‹L˜^‚µ‚½‚çÄ‹L˜^‚µ‚È‚¢
    }

    public void ResetLog()
    {
        logEnabled = false;
    }
}
