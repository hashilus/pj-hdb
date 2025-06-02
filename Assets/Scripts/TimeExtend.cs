using UnityEngine;
using System.Collections;

public class TimeExtendDisplay : MonoBehaviour
{
    [Header("TIME EXTEND 関連")]
    public GameObject extendGroup;     // TIME EXTEND文字列を含む親オブジェクト
    public TextMesh extendText;        // EXTEND数値 (TextMesh)
    
    [Header("時間管理")]
    public TimeManager timeManager;    // TimeManagerスクリプトへの参照

    [Header("演出タイミング")]
    public float addInterval = 0.1f;  // 1加算ごとの間隔
    public float showDelay = 1f;       // 表示後待機
    public float hideDelay = 1f;       // 加算完了後非表示までの待機

    private Coroutine extendRoutine;

    void Start()
    {
        if (extendGroup != null)
        {
            extendGroup.SetActive(false);
        }
    }

    /// <summary>
    /// 指定秒数をゆっくり加算しながらEXTEND表示を行う
    /// </summary>
    public void ShowExtend(int amount)
    {
        if(amount == 0)
        {
            return;
        }

        if (extendRoutine != null)
            StopCoroutine(extendRoutine);

        extendRoutine = StartCoroutine(HandleTimeExtend(amount));
    }

    private IEnumerator HandleTimeExtend(int amount)
    {
        extendGroup.SetActive(true);
        extendText.text = amount.ToString();

        yield return new WaitForSeconds(showDelay);

        int remaining = amount;
        while (remaining > 0)
        {
            timeManager.AddBonusTime(1f);
            remaining--;
            extendText.text = remaining.ToString();

            yield return new WaitForSeconds(addInterval);
        }

        yield return new WaitForSeconds(hideDelay);
        extendGroup.SetActive(false);
    }
}
