using System.Collections.Generic;
using UnityEngine;

public class CheckpointFireTriggerEvent : MonoBehaviour
{
    [System.Serializable]
    public class FireTriggerEntry
    {
        public Checkpoint targetCheckpoint;   // �Ώۂ̃`�F�b�N�|�C���g
        public int fireCountThreshold = 0;    // ���Ώ����F�c��΂����̐��ȉ�
        public GameObject triggerObject;      // �A�N�e�B�u������I�u�W�F�N�g
        public bool oneShot = true;           // ��x�������΂��邩
    }

    [Header("Fire Trigger ���X�g")]
    public List<FireTriggerEntry> fireTriggerEntries = new List<FireTriggerEntry>();

    private bool isActive = false;
    private List<bool> hasTriggered = new List<bool>();

    private void OnEnable()
    {
        Debug.Log($"[CheckpointFireTriggerEvent] {gameObject.name} �� �Ď��J�n");

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
                    Debug.Log($"[CheckpointFireTriggerEvent] {gameObject.name} �� Trigger ����: {entry.triggerObject.name} (FireCount={currentFireCount} <= {entry.fireCountThreshold})");
                }

                if (entry.oneShot)
                {
                    hasTriggered[i] = true;
                }
            }
        }
    }
}
