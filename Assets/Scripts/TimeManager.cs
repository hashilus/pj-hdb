using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class TimeManager : MonoBehaviour
{
    public float startTime = 300f;
    private float currentTime;

    private bool isCountingDown = false;

    public Action OnGameOver;
    public Action<float> OnTimeUpdated;

    public TextMesh dispTime;
    public TextMesh dispTimeDecimal;
    public TextMesh dispTime2;
    public TextMesh dispTimeDecimal2;

    public GameObject fireparticle;

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

        dispTime2.text = dispTime.text;
        dispTimeDecimal2.text = dispTimeDecimal.text;

        if (!isCountingDown) return;

        currentTime -= Time.deltaTime;

        OnTimeUpdated?.Invoke(currentTime);

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isCountingDown = false;
            Debug.Log("Game Over!");
            fireparticle.SetActive(true);
            FindObjectOfType<MaterialFader>().StartFade();
            OnGameOver?.Invoke();

            StartCoroutine(ReloadAfterDelay(10f)); // Ç±Ç±í«â¡ÅI
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
        currentTime += bonus; // Å©Ç±Ç±1â¡éZÇ∂Ç·Ç»Ç≠Çƒ bonus â¡éZÇµÇΩï˚Ç™é©ëR
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
