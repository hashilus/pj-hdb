using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CheckpointVoiceEvent : MonoBehaviour
{
    [System.Serializable]
    public class VoiceEntry
    {
        public float triggerTime;      // 何秒後に鳴らすか
        public AudioClip voiceClip;    // 鳴らすクリップ
        public GameObject activateObject; // ★再生時にActiveにするオブジェクト
    }

    [Header("Voiceイベントリスト")]
    public List<VoiceEntry> voiceEntries = new List<VoiceEntry>();

    private AudioSource audioSource;
    private float timer = 0f;
    private int currentIndex = 0;
    private bool isActive = false;

    private void OnEnable()
    {
        timer = 0f;
        currentIndex = 0;
        isActive = true;

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!isActive || voiceEntries.Count == 0)
            return;

        timer += Time.deltaTime;

        while (currentIndex < voiceEntries.Count && timer >= voiceEntries[currentIndex].triggerTime)
        {
            VoiceEntry entry = voiceEntries[currentIndex];

            if (entry.voiceClip != null)
            {
                audioSource.clip = entry.voiceClip;
                audioSource.Play();

                Debug.Log($"[CheckpointVoiceEvent] {gameObject.name} → Voice 再生開始 ( {entry.triggerTime} 秒経過 ) → {entry.voiceClip.name}");
            }

            // ★GameObjectをActive化
            if (entry.activateObject != null)
            {
                entry.activateObject.SetActive(true);
            }

            currentIndex++;
        }

        // 全部鳴らし終わったら停止も可能（必要なら）
        if (currentIndex >= voiceEntries.Count)
        {
            isActive = false;
            // ★もし自動的に Inactive にしたい場合はここで gameObject.SetActive(false); もOK
        }
    }
}
