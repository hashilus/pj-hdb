using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    public int checkPointNo; // チェックポイント番号

    public System.Action<Checkpoint> onCleared;
    private bool isCleared = false;

    public float clearTimeLimit = 10f;
    private Coroutine forceClearCoroutine;

    [Header("ライト制御を行うか？")]
    public bool controlLights = true;

    private List<FireController> fires = new List<FireController>();

    public float bonusTime; // このCPをクリアしたら追加される時間

    public TextMesh debugCountText; // デバッグ用の表示

    void Awake()
    {
        fires.Clear();

        foreach (var fire in GetComponentsInChildren<FireController>(true)) // ← Active=false も含めて取得
        {
            fires.Add(fire);
            fire.AssignCheckpoint(this); // FireController 側に自分を登録
        }

        SetLightActive(false);

        // ボーナスタイム取得
        bonusTime = SettingsManager.Instance.settings.checkpointExtendTimes[checkPointNo - 1];
    }

    public void NotifyFireExtinguished(FireController fc)
    {
        debugCountText.text = "残り" + GetAliveFireCount().ToString();
        CheckIfCleared();
    }

    public void ActivateCheckpoint()
    {
        debugCountText.text = "残り" + GetAliveFireCount().ToString();
        SetLightActive(true);

        if (forceClearCoroutine != null)
            StopCoroutine(forceClearCoroutine);

        forceClearCoroutine = StartCoroutine(ForceClearAfterDelay(clearTimeLimit));
    }

    private IEnumerator ForceClearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log($"{name}: 時間切れで強制消火を実行");

        foreach (var fire in fires)
        {
            if (fire != null && fire.life > 0f && fire.gameObject.activeInHierarchy)
            {
                fire.ForceExtinguish(); // ← FireController 側で用意
            }
        }

        CheckIfCleared(); // 念のためチェック
    }

    private void CheckIfCleared()
    {
        if (isCleared) return;

        float totalLife = 0f;

        foreach (var fire in fires)
        {
            if (fire != null && fire.gameObject.activeInHierarchy)
            {
                totalLife += Mathf.Max(0f, fire.life);
            }
        }

        if (totalLife <= 0f)
        {
            isCleared = true;
            Debug.Log($"{name}: 全FireのLifeが0。チェックポイントクリア！");
            onCleared?.Invoke(this);
            SetLightActive(false);
            ClearCheckpoint();
        }
    }

    public void SetLightActive(bool active)
    {
        if (!controlLights) return;

        foreach (Transform child in transform)
        {
            var light = child.GetComponentInChildren<Light>();
            if (light != null) light.enabled = active;
        }
    }

    void ClearCheckpoint()
    {
        TimeManager time = FindObjectOfType<TimeManager>();
        if (time != null)
        {
            time.StopCountdown();
            //time.AddBonusTime(bonusTime);
        }

        FindObjectOfType<TimeExtendDisplay>().ShowExtend((int)bonusTime);

        // 他のクリア処理
    }

    // 現在残っている火の個数を返す（単純な childCount は使わない！）
    public int GetCurrentFireCount()
    {
        int count = 0;

        foreach (var fire in fires)
        {
            if (fire != null)
            {
                count++;
            }
        }

        return count;
    }

    // 現在 Alive な Fire の数を返す
    public int GetAliveFireCount()
    {
        int count = 0;

        foreach (var fire in fires)
        {
            if (fire != null && fire.gameObject.activeInHierarchy && fire.life > 0f)
            {
                count++;
            }
        }

        return count;
    }
}
