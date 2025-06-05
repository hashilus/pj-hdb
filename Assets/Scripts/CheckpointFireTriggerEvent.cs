using System.Collections.Generic;
using UnityEngine;

public class CheckpointFireTriggerEvent : MonoBehaviour
{
    [System.Serializable]
    public class FireTriggerEntry
    {
        public Checkpoint targetCheckpoint;   // 対象のチェックポイント
        public int fireCountThreshold = 0;    // 発火条件：残り火がこの数以下
        public GameObject triggerObject;      // アクティブ化するオブジェクト
        public bool oneShot = true;           // 一度だけ発火するか
    }

    [Header("Fire Trigger リスト")]
    public List<FireTriggerEntry> fireTriggerEntries = new List<FireTriggerEntry>();

    private bool isActive = false;
    private List<bool> hasTriggered = new List<bool>();

    private void OnEnable()
    {
        Debug.Log($"[CheckpointFireTriggerEvent] {gameObject.name} → 監視開始");

        isActive = true;
        hasTriggered.Clear();
        for (int i = 0; i < fireTriggerEntries.Count; i++)
        {
            hasTriggered.Add(false);
        }
    }

    private void Update()
    {
        if (!isActive || fireTriggerEntries.Count == 0)
            return;

        for (int i = 0; i < fireTriggerEntries.Count; i++)
        {
            if (hasTriggered[i] && fireTriggerEntries[i].oneShot)
                continue;

            var entry = fireTriggerEntries[i];
            if (entry.targetCheckpoint == null)
                continue;

            int currentFireCount = entry.targetCheckpoint.GetCurrentFireCount();

            if (currentFireCount <= entry.fireCountThreshold)
            {
                if (entry.triggerObject != null)
                {
                    entry.triggerObject.SetActive(true);
                    Debug.Log($"[CheckpointFireTriggerEvent] {gameObject.name} → Trigger 発動: {entry.triggerObject.name} (FireCount={currentFireCount} <= {entry.fireCountThreshold})");
                }

                if (entry.oneShot)
                {
                    hasTriggered[i] = true;
                }
            }
        }
    }
}
