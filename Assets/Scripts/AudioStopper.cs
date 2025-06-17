using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioStopper : MonoBehaviour
{
    //登録した全てのAudioSourceを停止するスクリプト

    public AudioSource[] audioSources;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i] != null)
            {
                audioSources[i].Stop();
                audioSources[i].enabled = false; // AudioSourceを無効化
            }
        }
    }

}
