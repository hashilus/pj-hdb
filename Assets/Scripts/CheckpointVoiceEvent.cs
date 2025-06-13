using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CheckpointVoiceEvent : MonoBehaviour
{
    [System.Serializable]
    public class VoiceEntry
    {
        public float triggerTime;      // ���b��ɖ炷��
        public AudioClip voiceClip;    // �炷�N���b�v
        public GameObject activateObject; // ���Đ�����Active�ɂ���I�u�W�F�N�g
    }

    [Header("Voice�C�x���g���X�g")]
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

                Debug.Log($"[CheckpointVoiceEvent] {gameObject.name} �� Voice �Đ��J�n ( {entry.triggerTime} �b�o�� ) �� {entry.voiceClip.name}");
            }

            // ��GameObject��Active��
            if (entry.activateObject != null)
            {
                entry.activateObject.SetActive(true);
            }

            currentIndex++;
        }

        // �S���炵�I��������~���\�i�K�v�Ȃ�j
        if (currentIndex >= voiceEntries.Count)
        {
            isActive = false;
            // �����������I�� Inactive �ɂ������ꍇ�͂����� gameObject.SetActive(false); ��OK
        }
    }
}
