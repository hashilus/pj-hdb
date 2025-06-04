using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    [Header("ステージ構成")]
    public List<Checkpoint> checkpoints;

    [Header("導入Timeline")]
    public PlayableDirector pre_timeline;

    [Header("メインTimeline")]
    public PlayableDirector timeline;

    private int currentIndex = 0;

    public bool isReset;
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

        // Timelineイベント登録（Playはしない！）
        if (pre_timeline != null)
        {
            pre_timeline.stopped += OnPreTimelineStopped;
            pre_timeline.time = 0.0;
            pre_timeline.Evaluate();
            pre_timeline.Stop();
        }

        if (timeline != null)
        {
            timeline.stopped += OnMainTimelineStopped;
            timeline.time = 0.0;
            timeline.Evaluate();
            timeline.Stop();
        }

    }

    private void Update()
    {
        // Rキーでシーンリロード
        if (Input.GetKeyDown(KeyCode.R) || isReset)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // ESCキーでアプリ終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    // ★ 外部（TitleController）から呼ばれる
    public void StartGame()
    {
        if (pre_timeline != null)
        {
            Debug.Log("StageController → 導入Timeline開始 (StartGame呼び出し)");
            pre_timeline.Play();
        }
    }

    // Timeline内のSignal Emitterから呼ばれる
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
            cp.SetLightActive(true);
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
            // エンディング（クリア後Timeline）
            Debug.Log("すべてのチェックポイントをクリアしました！");
            timeline.Play();
        }
    }

    // 導入Timelineが終わったら → メインTimeline開始
    private void OnPreTimelineStopped(PlayableDirector director)
    {
        Debug.Log("導入Timeline終了 → メインTimeline開始");
        timeline?.Play();
    }

    // メインTimeline終了時（今は何もしないが拡張可）
    private void OnMainTimelineStopped(PlayableDirector director)
    {
        Debug.Log("メインTimeline終了");
    }

    // クリア後Timeline終了時 → リロードは行わない
    private void OnAfterTimelineStopped(PlayableDirector director)
    {
        Debug.Log("クリア後Timeline終了 → ここでタイトルに戻すなどの処理を入れてOK");

        // ここは今後 Title に戻す／UI表示する場所になる
        // 例: titleController.ResetTitle() を呼ぶ など
    }

    public void ReloadScene()
    {
        Debug.Log("StageController → ReloadScene() 実行 → シーンリセット");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
