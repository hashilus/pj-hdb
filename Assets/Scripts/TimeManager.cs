using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public float startTime = 300f; // 初期残り時間（秒）
    private float currentTime;

    private bool isCountingDown = false;

    public Action OnGameOver;
    public Action<float> OnTimeUpdated; // UI表示更新用

    public TextMesh dispTime;
    public TextMesh dispTimeDecimal;

    void Start()
    {
        currentTime = startTime;
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

        if (OnTimeUpdated != null)
            OnTimeUpdated(currentTime);

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isCountingDown = false;
            Debug.Log("Game Over!");
            FindObjectOfType<MaterialFader>().StartFade();
            OnGameOver?.Invoke();
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
        currentTime++;
        if (OnTimeUpdated != null)
            OnTimeUpdated(currentTime);
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }
}
