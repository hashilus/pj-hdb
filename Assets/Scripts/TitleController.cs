using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    [Header("タイトル系オブジェクト")]
    public GameObject title_objects;

    [Header("UI系オブジェクト")]
    public GameObject ui_objects;

    public StageController stageController;

    public bool isStarting = false;

    public int player1StartCount;
    public int player2StartCount;

    public int startupnumber;

    public TextMesh debug_1pHit;
    public TextMesh debug_2pHit;

    float gameStartingTimer = 10.0f;
    private bool startTriggered = false; // ★ Play() 二重呼び出し防止用

    public GameObject player1Ready;
    public GameObject player2Ready;

    public GameObject player1Timer;
    public GameObject player2Timer;

    bool start1P_Confirmed;
    bool start2P_Confirmed;

    [Header("メインカメラオブジェクト")]
    public GameObject camPlayer;
    public Transform camPosition;

    bool isPlayed = false;

    public AudioSource mainBGM;

    public GameObject playerRoot;

    public GameObject calibrationUI;
    private Coroutine startDelayCoroutine; // 追加: スタート遅延用

    public AudioClip startSound;


    public GameObject player1root;
    public GameObject player2root;

    public MaterialFader materialFader;

    public GameLogManager gameLogManager;

    void Start()
    {
        if (SettingsManager.Instance.settings.debugMode)
        {
            debug_1pHit.gameObject.SetActive(true);
            debug_2pHit.gameObject.SetActive(true);
        }
        else
        {
            debug_1pHit.gameObject.SetActive(false);
            debug_2pHit.gameObject.SetActive(false);
        }
        title_objects.SetActive(true);
        ui_objects.SetActive(false);
        if ((Input.GetKeyDown(KeyCode.Space) || isStarting) && !startTriggered)
        calibrationUI.SetActive(true);

        //materialFader.FadeIn(); // フェードインを有効にする
    }

    void Update()
    {
        if (!isPlayed)
        {
            //カメラ位置をタイトル画面位置に固定
            camPlayer.transform.position = camPosition.position;
            camPlayer.transform.rotation = camPosition.rotation;
        }

        // ★ プレイヤー判定ロジックはそのままでOK
        if (player1StartCount > startupnumber)
        {
            player2Timer.SetActive(true);
            player1Ready.SetActive(true);
            gameStartingTimer -= Time.deltaTime;
            if (!start1P_Confirmed)
            {
                player2StartCount = startupnumber - 1;
                GetComponent<AudioSource>().PlayOneShot(startSound);

            }
            start1P_Confirmed = true;
        }

        if (player2StartCount > startupnumber)
        {
            player1Timer.SetActive(true);
            player2Ready.SetActive(true);
            gameStartingTimer -= Time.deltaTime;
            if (!start2P_Confirmed)
            {
                player1StartCount = startupnumber - 1;
                GetComponent<AudioSource>().PlayOneShot(startSound);
            }
            start2P_Confirmed = true;
        }

        if (player1StartCount > startupnumber && player2StartCount > startupnumber)
        {
            if (startDelayCoroutine == null && !startTriggered)
            {
                startDelayCoroutine = StartCoroutine(StartGameAfterDelay(1f));
            }
        }

        if (gameStartingTimer < 0)
        {
            isStarting = true;
        }

        debug_1pHit.text = player1StartCount.ToString();
        debug_2pHit.text = player2StartCount.ToString();

        if (!isPlayed) {
            playerRoot.SetActive(true);
            playerRoot.transform.localPosition = Vector3.zero;
        }

        // ★ スタート処理を一度だけ行うようにする

        if (Input.GetKeyDown(KeyCode.Space))
        {
            start1P_Confirmed = true;
        }

        if ((Input.GetKeyDown(KeyCode.Space) || isStarting) && !startTriggered)
        {
            startTriggered = true;
            isPlayed = true;

            // ① Title画面を閉じる
            title_objects.SetActive(false);
            ui_objects.SetActive(true);
            //カメラ位置をリセット
            camPlayer.transform.localPosition = Vector3.zero;
            camPlayer.transform.localRotation = Quaternion.identity;

            // ② StageController に pre_timeline 再生を任せる
            if (stageController != null && stageController.pre_timeline != null)
            {
                Debug.Log("TitleController → StageController.pre_timeline.Play() 呼び出し");
                stageController.pre_timeline.Play();
            }

            // ③ 変数リセット
            isStarting = false;
            gameStartingTimer = 10f;
            player1StartCount = 0;
            player2StartCount = 0;

            //プレイヤーを確定する
            player1root.SetActive(start1P_Confirmed);
            player2root.SetActive(start2P_Confirmed);

            //SettingsInstance にプレイヤー数を設定
            if (start1P_Confirmed && start2P_Confirmed)
            {
                SettingsManager.Instance.playingPlayerNumber = 2;
            }
            else {

                SettingsManager.Instance.playingPlayerNumber = 1;
            }

            Debug.Log($"TitleController: プレイヤー数設定完了 → {SettingsManager.Instance.playingPlayerNumber}人");

            gameLogManager.RecordStartTime();

            mainBGM.Play();

        }
    }
    private IEnumerator StartGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isStarting = true;
    }
}
