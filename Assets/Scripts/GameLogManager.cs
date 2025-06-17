using System;
using System.IO;
using UnityEngine;

public class GameLogManager : MonoBehaviour
{
    private float stage1ClearTime = -1f;
    private float stage2ClearTime = -1f;
    private float stage3ClearTime = -1f;
    private float stage4ClearTime = -1f;
    private float stage5ClearTime = -1f;
    private float stage6ClearTime = -1f;
    private string rank = "";
    private bool logEnabled = true;

    private float startTime;

    private string startTimeStr;

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
            case 4:
                stage4ClearTime = remainingTime;
                break;
            case 5:
                stage5ClearTime = remainingTime;
                break;
            case 6:
                stage6ClearTime = remainingTime;
                break;        }
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

    string s1 = stage1ClearTime >= 0 ? stage1ClearTime.ToString("F2") : "";
    string s2 = stage2ClearTime >= 0 ? stage2ClearTime.ToString("F2") : "";
    string s3 = stage3ClearTime >= 0 ? stage3ClearTime.ToString("F2") : "";
    string s4 = stage4ClearTime >= 0 ? stage4ClearTime.ToString("F2") : "";
    string s5 = stage5ClearTime >= 0 ? stage5ClearTime.ToString("F2") : "";
    string s6 = stage6ClearTime >= 0 ? stage6ClearTime.ToString("F2") : "";

    // プレイ人数を取得
    int playerNum = 1;
    if (SettingsManager.Instance != null)
        playerNum = SettingsManager.Instance.playingPlayerNumber;

    // プレイ人数を含めてCSV行を作成
        string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
        date, startTimeStr, playerNum, s1, s2, s3, s4, s5, s6, rank);

    string logDir;
#if UNITY_EDITOR
    logDir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "PlayLog");
#else
    logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PlayLog");
#endif

    if (!Directory.Exists(logDir))
    {
        Directory.CreateDirectory(logDir);
    }

    string fileName = DateTime.Now.ToString("yyyyMMdd") + ".csv";
    string fullPath = Path.Combine(logDir, fileName);

    File.AppendAllText(fullPath, line + "\n");

    Debug.Log("[GameLogManager] Log written: " + line + " (" + fullPath + ")");

    logEnabled = false; // 一度記録したら再記録しない
}



    public void ResetLog()
    {
        logEnabled = false;
    }
    public void RecordStartTime()
    {
        startTimeStr = DateTime.Now.ToString("HH:mm:ss");
        Debug.Log($"[GameLogManager] ゲームスタート時刻記録: {startTimeStr}");
    }
}
