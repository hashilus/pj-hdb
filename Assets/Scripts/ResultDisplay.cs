using UnityEngine;

public class ResultDisplay : MonoBehaviour
{
    [Header("残タイム表示用 TextMesh")]
    public TextMesh timeText;

    [Header("ランク用 GameObjects")]
    public GameObject rankSObject;
    public GameObject rankAObject;
    public GameObject rankBObject;
    public GameObject rankCObject;

    public GameLogManager gameLogManager;

    private void OnEnable()
    {
        Debug.Log("ResultDisplay → 起動");

        // �@ 残タイムを取得
        TimeManager timeManager = FindObjectOfType<TimeManager>();
        if (timeManager == null)
        {
            Debug.LogError("ResultDisplay: TimeManager が見つかりません！");
            return;
        }

        float remainingTime = timeManager.GetCurrentTime();
        Debug.Log($"ResultDisplay: 残タイム = {remainingTime:F1} 秒");

        // �A 残タイムを TextMesh に表示
        if (timeText != null)
        {
            timeText.text = $"{remainingTime:F1}";
        }

        // �B ランク閾値を取得
        var thresholds = SettingsManager.Instance.settings.resultThresholds;

        // �C ランク判定 → GameObject Active 切り替え
        if (remainingTime >= thresholds.rankS)
        {
            SetRank("S");
            gameLogManager.SetRank("S");
        }
        else if (remainingTime >= thresholds.rankA)
        {
            SetRank("A");
            gameLogManager.SetRank("A");
        }
        else if (remainingTime >= thresholds.rankB)
        {
            SetRank("B");
            gameLogManager.SetRank("B");
        }
        else
        {
            SetRank("C");
            gameLogManager.SetRank("C");
        }

        gameLogManager.WriteLog();
    }

    private void SetRank(string rank)
    {
        Debug.Log($"ResultDisplay: ランク = {rank}");

        // 全ランクを初期OFF
        if (rankSObject != null) rankSObject.SetActive(false);
        if (rankAObject != null) rankAObject.SetActive(false);
        if (rankBObject != null) rankBObject.SetActive(false);
        if (rankCObject != null) rankCObject.SetActive(false);

        // 対象ランクだけON
        switch (rank)
        {
            case "S":
                if (rankSObject != null) rankSObject.SetActive(true);
                break;
            case "A":
                if (rankAObject != null) rankAObject.SetActive(true);
                break;
            case "B":
                if (rankBObject != null) rankBObject.SetActive(true);
                break;
            case "C":
                if (rankCObject != null) rankCObject.SetActive(true);
                break;
        }
    }
}
