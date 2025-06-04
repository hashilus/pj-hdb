using System.Collections;
using UnityEngine;



[RequireComponent(typeof(AudioSource))]
public class RandomVoiceLooper : MonoBehaviour
{
    [Header("�Đ�����{�C�X�N���b�v (������)")]
    public AudioClip[] voiceClips;

    [Header("�Đ��Ԋu (�b)")]
    public float minInterval = 5f;
    public float maxInterval = 10f;

    private AudioSource audioSource;
    private Coroutine playLoopCoroutine;

    private void OnEnable()
    {
        Debug.Log($"[RandomVoiceLooper] {gameObject.name} �� ���[�v�J�n");

        audioSource = GetComponent<AudioSource>();

        if (voiceClips != null && voiceClips.Length > 0)
        {
            playLoopCoroutine = StartCoroutine(PlayLoop());
        }
        else
        {
            Debug.LogWarning($"[RandomVoiceLooper] {gameObject.name} �� voiceClips ���ݒ�B���[�v�J�n����");
        }
    }

    private void OnDisable()
    {
        if (playLoopCoroutine != null)
        {
            StopCoroutine(playLoopCoroutine);
            playLoopCoroutine = null;
        }

        Debug.Log($"[RandomVoiceLooper] {gameObject.name} �� ���[�v��~");
    }

    private IEnumerator PlayLoop()
    {
        while (true)
        {
            // �����_���Ԋu
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // �����_��Clip�I��
            if (voiceClips.Length > 0)
            {
                int index = Random.Range(0, voiceClips.Length);
                AudioClip clip = voiceClips[index];

                if (clip != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();

                    Debug.Log($"[RandomVoiceLooper] {gameObject.name} �� Voice �Đ�: {clip.name}");
                }
            }
        }
    }
}
