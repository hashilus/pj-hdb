using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] voiceClips;

    public void PlayVoice(int voiceNumber)
    {
        if (voiceNumber < 0 || voiceNumber >= voiceClips.Length)
        {
            Debug.LogWarning($"Voice number {voiceNumber} is out of range!");
            return;
        }

        audioSource.clip = voiceClips[voiceNumber];
        audioSource.Play();
    }
}
