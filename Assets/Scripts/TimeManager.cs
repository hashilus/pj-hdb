using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class TimeManager : MonoBehaviour
{
    public float startTime = 300f;
    public float currentTime;

    private bool isCountingDown = false;

    public Action OnGameOver;
    public Action<float> OnTimeUpdated;

    public TextMesh dispTime;
    public TextMesh dispTimeDecimal;

    public GameObject fireparticle;

    public GameLogManager gameLogManager;

    public MaterialFader materialFader;
    public MaterialFader materialFaderBlack;

    void Start()
    {
        currentTime = SettingsManager.Instance.settings.initialTime;
        isCountingDown = false;
    }

    void Update()
    {
        dispTime.text = currentTime.ToString("000");
        string timeStr = currentTime.ToString("F1");
        string[] parts = timeStr.Split('.');
        dispTimeDecimal.text = parts[1];

        if (!isCountingDown) return;

        currentTime -= Time.deltaTime;

        OnTimeUpdated?.Invoke(currentTime);

        if (currentTime <= 0f)
        {
            gameLogManager.SetRank("OVER");
            gameLogManager.WriteLog();
            currentTime = 0f;
            isCountingDown = false;
            Debug.Log("Game Over!");
            fireparticle.SetActive(true);
            materialFader.FadeIn(); // �ʏ�t�F�[�h
            OnGameOver?.Invoke();

            StartCoroutine(ReloadAfterDelay()); // �����Ȃ��ɕύX
        }
    }

    public void StartCountdown()
    {
        isCountingDown = true;
    }

    public void StopCountdown()
    {
        isCountingDown = false;
    }

    public void AddBonusTime(float bonus)
    {
        currentTime += bonus; // ������1���Z����Ȃ��� bonus ���Z�����������R
        OnTimeUpdated?.Invoke(currentTime);
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    private System.Collections.IEnumerator ReloadAfterDelay()
    {
        // 10�b�ҋ@
        yield return new WaitForSeconds(10f);

        // �u���b�N�t�F�[�h�A�E�g�J�n
        if (materialFaderBlack != null)
        {
            materialFaderBlack.FadeIn();
        }

        // �����3�b�ҋ@
        yield return new WaitForSeconds(3f);

        // �V�[�������[�h
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
