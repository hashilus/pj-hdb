using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StageController : MonoBehaviour
{
    [Header("ステージ構成")]
    public List<Checkpoint> checkpoints;

    [Header("Timeline")]
    public PlayableDirector timeline;

    private int currentIndex = 0;

    void Start()
    {
        // 最初のチェックポイントのライトだけONにする
        for (int i = 0; i < checkpoints.Count; i++)
        {
            checkpoints[i].SetLightActive(false); // 念のため全OFF
        }

        if (checkpoints.Count > 0)
        {
            checkpoints[0].SetLightActive(true);
        }

        if (timeline != null)
        {
            timeline.time = 0.0;
            timeline.Evaluate();     // 即座に初期フレームへ反映
            timeline.Stop();         // 再生状態に入らないようにする
        }
    }

    private void Update()
    {
        // Rキーでシーンリロード
        if (Input.GetKeyDown(KeyCode.R)) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        // ESCキーでアプリ終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }



    }

    /// <summary>
    /// Timeline内のSignal Emitterから呼ばれる（インスペクタで指定）
    /// </summary>
    public void OnCheckpointReached()
    {
        Debug.Log($"チェックポイント {currentIndex} に到達");
        timeline.Pause();
        checkpoints[currentIndex].ActivateCheckpoint();

        TimeManager time = FindObjectOfType<TimeManager>();
        if (time != null)
        {
            time.StartCountdown();
        }

        if (currentIndex < checkpoints.Count)
        {
            var cp = checkpoints[currentIndex];
            cp.onCleared += OnCheckpointCleared;
            cp.SetLightActive(true); // 忘れずON
        }
    }

    private void OnCheckpointCleared(Checkpoint cp)
    {
        Debug.Log($"StageController: {cp.name} のクリアを受信");

        cp.onCleared -= OnCheckpointCleared;
        cp.SetLightActive(false);
        currentIndex++;

        TimeManager time = FindObjectOfType<TimeManager>();
        if (time != null)
        {
            time.StopCountdown();
        }

        if (currentIndex < checkpoints.Count)
        {
            checkpoints[currentIndex].SetLightActive(true);
            timeline.Play();
            Debug.Log("Timeline 再開！");
        }
        else
        {
            Debug.Log("すべてのチェックポイントをクリアしました！");
        }
    }
}
