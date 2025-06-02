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

    public bool isStarting = false; // �X�^�[�g�{�^���������ꂽ���ǂ���

    public int player1StartCount;
    public int player2StartCount;

    public int startupnumber;

    public TextMesh debug_1pHit;
    public TextMesh debug_2pHit;

    float gameStartingTimer = 10.0f;

    public GameObject player1Ready;
    public GameObject player2Ready;

    public GameObject player1Timer;
    public GameObject player2Timer;

    // Start is called before the first frame update
    void Start()
    {
        title_objects.SetActive(true);
        ui_objects.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

        if (player1StartCount > startupnumber)
        {
            player2Timer.SetActive(true);
            player1Ready.SetActive(true);
            gameStartingTimer -= Time.deltaTime; // �^�C�}�[�����炷
            player2StartCount = startupnumber - 1;
        }

        if (player2StartCount > startupnumber)
        {
            player1Timer.SetActive(true);
            player2Ready.SetActive(true);
            gameStartingTimer -= Time.deltaTime; // �^�C�}�[�����炷
            player1StartCount = startupnumber - 1;
        }

        if(player1StartCount > startupnumber && player2StartCount > startupnumber)
        {
            isStarting = true;
        }

        if(gameStartingTimer < 0)
        {
            isStarting = true;
        }

        debug_1pHit.text = player1StartCount.ToString();
        debug_2pHit.text = player2StartCount.ToString();


        if (Input.GetKeyDown(KeyCode.Space) || isStarting)
        {
            stageController.timeline.Play();
            title_objects.SetActive(false);
            ui_objects.SetActive(true);
            isStarting = false; 
            gameStartingTimer�@=10f; // �^�C�}�[�����Z�b�g
            player1StartCount = 0; // �X�^�[�g�J�E���g�����Z�b�g
            player2StartCount = 0; // �X�^�[�g�J�E���g�����Z�b�g
        }

    }
}
