using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    public System.Action<Checkpoint> onCleared;
    private bool isCleared = false;
    
    public float clearTimeLimit = 10f;
    private Coroutine forceClearCoroutine;


    [Header("ライト制御を行うか？")]
    public bool controlLights = true;

    private List<FireController> fires = new List<FireController>();

    public float bonusTime = 10f; // このCPをクリアしたら追加される時間

    public TextMesh debugCountText; // デバッグ用の表示

    int firesCount = 0;

    void Awake()
    {
        fires.Clear();
        foreach (var fire in GetComponentsInChildren<FireController>())
        {
            fires.Add(fire);
            fire.AssignCheckpoint(this); // FireController 側に自分を登録
        }
        firesCount = fires.Count;

        SetLightActive(false);
    }

    public void NotifyFireExtinguished(FireController fc)
    {
        // 呼ばれるたびにチェック
        firesCount--;
        debugCountText.text = "残り" + firesCount.ToString();
        CheckIfCleared();
    }


    public void ActivateCheckpoint()
    {
        debugCountText.text = "残り" + fires.Count.ToString();
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
            if (fire.life > 0)
            {
                fire.ForceExtinguish(); // ← FireController側で用意（後述）
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
            totalLife += Mathf.Max(0f, fire.life);
        }

        if (totalLife <= 0f)
        {
            isCleared = true;
            Debug.Log($"{name}: 全FireのLifeが0。チェックポイントクリア！");
            onCleared?.Invoke(this);
            SetLightActive(false);
            ClearCheckpoint(); // クリア処理
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
            time.AddBonusTime(bonusTime);
        }

        FindObjectOfType<TimeExtendDisplay>().ShowExtend((int)bonusTime);

        // 他のクリア処理
    }


}
