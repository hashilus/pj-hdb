using System.Collections;
using UnityEngine;



[RequireComponent(typeof(AudioSource))]
public class RandomVoiceLooper : MonoBehaviour
{
    [Header("再生するボイスクリップ (複数可)")]
    public AudioClip[] voiceClips;

    [Header("再生間隔 (秒)")]
    public float minInterval = 5f;
    public float maxInterval = 10f;

    private AudioSource audioSource;
    private Coroutine playLoopCoroutine;

    private void OnEnable()
    {
        Debug.Log($"[RandomVoiceLooper] {gameObject.name} → ループ開始");

        audioSource = GetComponent<AudioSource>();

        if (voiceClips != null && voiceClips.Length > 0)
        {
            playLoopCoroutine = StartCoroutine(PlayLoop());
        }
        else
        {
            Debug.LogWarning($"[RandomVoiceLooper] {gameObject.name} → voiceClips 未設定。ループ開始せず");
        }
    }

    private void OnDisable()
    {
        if (playLoopCoroutine != null)
        {
            StopCoroutine(playLoopCoroutine);
            playLoopCoroutine = null;
        }

        Debug.Log($"[RandomVoiceLooper] {gameObject.name} → ループ停止");
    }

    private IEnumerator PlayLoop()
    {
        while (true)
        {
            // ランダム間隔
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // ランダムClip選択
            if (voiceClips.Length > 0)
            {
                int index = Random.Range(0, voiceClips.Length);
                AudioClip clip = voiceClips[index];

                if (clip != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();

                    Debug.Log($"[RandomVoiceLooper] {gameObject.name} → Voice 再生: {clip.name}");
                }
            }
        }
    }
}
