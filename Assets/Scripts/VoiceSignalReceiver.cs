using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class VoiceSignalReceiver : MonoBehaviour, INotificationReceiver
{
    public VoiceManager voiceManager;

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification is VoiceSignalEmitter signalEmitter)
        {
            int voiceNumber = signalEmitter.voiceNumber;
            Debug.Log($"VoiceSignal received: Voice {voiceNumber}");

            if (voiceManager != null)
            {
                voiceManager.PlayVoice(voiceNumber);
            }
        }
    }
}
