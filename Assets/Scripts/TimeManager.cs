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
            materialFader.FadeIn(); // フェードアウトを有効にする
            OnGameOver?.Invoke();

            StartCoroutine(ReloadAfterDelay(10f)); // ここ追加！
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
        currentTime += bonus; // ←ここ1加算じゃなくて bonus 加算した方が自然
        OnTimeUpdated?.Invoke(currentTime);
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    private System.Collections.IEnumerator ReloadAfterDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
