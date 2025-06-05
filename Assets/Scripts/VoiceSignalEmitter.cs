using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class VoiceSignalEmitter : Marker, INotification
{
    public PropertyName id => new PropertyName();
    public int voiceNumber;
}
