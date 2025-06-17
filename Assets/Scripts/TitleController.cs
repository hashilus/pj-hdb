using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    [Header("�^�C�g���n�I�u�W�F�N�g")]
    public GameObject title_objects;

    [Header("UI�n�I�u�W�F�N�g")]
    public GameObject ui_objects;

    public StageController stageController;

    public bool isStarting = false;

    public int player1StartCount;
    public int player2StartCount;

    public int startupnumber;

    public TextMesh debug_1pHit;
    public TextMesh debug_2pHit;

    float gameStartingTimer = 10.0f;
    private bool startTriggered = false; // �� Play() ��d�Ăяo���h�~�p

    public GameObject player1Ready;
    public GameObject player2Ready;

    public GameObject player1Timer;
    public GameObject player2Timer;

    bool start1P_Confirmed;
    bool start2P_Confirmed;

    [Header("���C���J�����I�u�W�F�N�g")]
    public GameObject camPlayer;
    public Transform camPosition;

    bool isPlayed = false;

    public AudioSource mainBGM;

    public GameObject playerRoot;

    public GameObject calibrationUI;
    private Coroutine startDelayCoroutine; // �ǉ�: �X�^�[�g�x���p

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

        //materialFader.FadeIn(); // �t�F�[�h�C����L���ɂ���
    }

    void Update()
    {
        if (!isPlayed)
        {
            //�J�����ʒu���^�C�g����ʈʒu�ɌŒ�
            camPlayer.transform.position = camPosition.position;
            camPlayer.transform.rotation = camPosition.rotation;
        }

        // �� �v���C���[���胍�W�b�N�͂��̂܂܂�OK
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

        // �� �X�^�[�g��������x�����s���悤�ɂ���

        if (Input.GetKeyDown(KeyCode.Space))
        {
            start1P_Confirmed = true;
        }

        if ((Input.GetKeyDown(KeyCode.Space) || isStarting) && !startTriggered)
        {
            startTriggered = true;
            isPlayed = true;

            // �@ Title��ʂ����
            title_objects.SetActive(false);
            ui_objects.SetActive(true);
            //�J�����ʒu�����Z�b�g
            camPlayer.transform.localPosition = Vector3.zero;
            camPlayer.transform.localRotation = Quaternion.identity;

            // �A StageController �� pre_timeline �Đ���C����
            if (stageController != null && stageController.pre_timeline != null)
            {
                Debug.Log("TitleController �� StageController.pre_timeline.Play() �Ăяo��");
                stageController.pre_timeline.Play();
            }

            // �B �ϐ����Z�b�g
            isStarting = false;
            gameStartingTimer = 10f;
            player1StartCount = 0;
            player2StartCount = 0;

            //�v���C���[���m�肷��
            player1root.SetActive(start1P_Confirmed);
            player2root.SetActive(start2P_Confirmed);

            //SettingsInstance �Ƀv���C���[����ݒ�
            if (start1P_Confirmed && start2P_Confirmed)
            {
                SettingsManager.Instance.playingPlayerNumber = 2;
            }
            else {

                SettingsManager.Instance.playingPlayerNumber = 1;
            }

            Debug.Log($"TitleController: �v���C���[���ݒ芮�� �� {SettingsManager.Instance.playingPlayerNumber}�l");

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
