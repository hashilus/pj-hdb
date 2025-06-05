using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    [Header("�X�e�[�W�\��")]
    public List<Checkpoint> checkpoints;

    [Header("����Timeline")]
    public PlayableDirector pre_timeline;

    [Header("���C��Timeline")]
    public PlayableDirector timeline;

    private int currentIndex = 0;

    public bool isReset;
    void Start()
    {
        // �ŏ��̃`�F�b�N�|�C���g�̃��C�g����ON�ɂ���
        for (int i = 0; i < checkpoints.Count; i++)
        {
            checkpoints[i].SetLightActive(false); // �O�̂��ߑSOFF
        }

        if (checkpoints.Count > 0)
        {
            checkpoints[0].SetLightActive(true);
        }

        // Timeline�C�x���g�o�^�iPlay�͂��Ȃ��I�j
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
        // R�L�[�ŃV�[�������[�h
        if (Input.GetKeyDown(KeyCode.R) || isReset)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // ESC�L�[�ŃA�v���I��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    // �� �O���iTitleController�j����Ă΂��
    public void StartGame()
    {
        if (pre_timeline != null)
        {
            Debug.Log("StageController �� ����Timeline�J�n (StartGame�Ăяo��)");
            pre_timeline.Play();
        }
    }

    // Timeline����Signal Emitter����Ă΂��
    public void OnCheckpointReached()
    {
        Debug.Log($"�`�F�b�N�|�C���g {currentIndex} �ɓ��B");

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
        Debug.Log($"StageController: {cp.name} �̃N���A����M");

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
            Debug.Log("Timeline �ĊJ�I");
        }
        else
        {
            // �G���f�B���O�i�N���A��Timeline�j
            Debug.Log("���ׂẴ`�F�b�N�|�C���g���N���A���܂����I");
            timeline.Play();
        }
    }

    // ����Timeline���I������� �� ���C��Timeline�J�n
    private void OnPreTimelineStopped(PlayableDirector director)
    {
        Debug.Log("����Timeline�I�� �� ���C��Timeline�J�n");
        timeline?.Play();
    }

    // ���C��Timeline�I�����i���͉������Ȃ����g���j
    private void OnMainTimelineStopped(PlayableDirector director)
    {
        Debug.Log("���C��Timeline�I��");
    }

    // �N���A��Timeline�I���� �� �����[�h�͍s��Ȃ�
    private void OnAfterTimelineStopped(PlayableDirector director)
    {
        Debug.Log("�N���A��Timeline�I�� �� �����Ń^�C�g���ɖ߂��Ȃǂ̏���������OK");

        // �����͍��� Title �ɖ߂��^UI�\������ꏊ�ɂȂ�
        // ��: titleController.ResetTitle() ���Ă� �Ȃ�
    }

    public void ReloadScene()
    {
        Debug.Log("StageController �� ReloadScene() ���s �� �V�[�����Z�b�g");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
