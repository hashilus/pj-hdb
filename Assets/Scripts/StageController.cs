using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StageController : MonoBehaviour
{
    [Header("�X�e�[�W�\��")]
    public List<Checkpoint> checkpoints;

    [Header("Timeline")]
    public PlayableDirector timeline;

    private int currentIndex = 0;

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

        if (timeline != null)
        {
            timeline.time = 0.0;
            timeline.Evaluate();     // �����ɏ����t���[���֔��f
            timeline.Stop();         // �Đ���Ԃɓ���Ȃ��悤�ɂ���
        }
    }

    private void Update()
    {
        // R�L�[�ŃV�[�������[�h
        if (Input.GetKeyDown(KeyCode.R)) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        // ESC�L�[�ŃA�v���I��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }



    }

    /// <summary>
    /// Timeline����Signal Emitter����Ă΂��i�C���X�y�N�^�Ŏw��j
    /// </summary>
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
            cp.SetLightActive(true); // �Y�ꂸON
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
            Debug.Log("���ׂẴ`�F�b�N�|�C���g���N���A���܂����I");
        }
    }
}
