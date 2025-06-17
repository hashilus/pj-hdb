using UnityEngine;

public class PanicOverlay : MonoBehaviour
{
    [Header("設定")]
    public MeshRenderer meshRenderer; // 赤いグラデ素材を貼ったQuadなどのMeshRenderer
    public float startTimeThreshold = 10f; // 発動残り時間（秒）
    public float maxAlpha = 0.8f;           // 最も不透明なときのAlpha
    public float minAlpha = 0.1f;           // 最も透明なときのAlpha
    public float minSpeed = 1f;             // 揺れ始めの速度
    public float maxSpeed = 5f;             // 残り0秒のときの速度

    [Header("警告音設定")]
    public AudioSource audioSource;
    public AudioClip[] countdownClips = new AudioClip[10]; // 10秒〜1秒用
    private bool[] hasPlayed = new bool[10];

    private Material materialInstance;
    private float timeLeft = 0f;
    private bool isActive = false;
    private TimeManager timeManager;

    void Start()
    {
        if (meshRenderer != null)
        {
            materialInstance = meshRenderer.material;
            Color color = materialInstance.color;
            color.a = 0f; // 初期状態で完全に透明にする
            materialInstance.color = color;
        }

        timeManager = FindObjectOfType<TimeManager>();
    }

    void Update()
    {
        if (materialInstance == null || timeManager == null) return;

        timeLeft = timeManager.GetCurrentTime();

        if (timeLeft <= startTimeThreshold)
        {
            isActive = true;
            float t = Mathf.InverseLerp(startTimeThreshold, 0f, timeLeft); // 0→1
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
            float speed = Mathf.Lerp(minSpeed, maxSpeed, t);

            float wave = Mathf.Sin(Time.time * speed) * 0.5f + 0.5f; // 0~1
            float finalAlpha = alpha * wave;

            Color color = materialInstance.color;
            color.a = finalAlpha;
            materialInstance.color = color;

            int secondsLeft = Mathf.CeilToInt(timeLeft);
            if (secondsLeft <= 10 && secondsLeft >= 1)
            {
                int index = 10 - secondsLeft;
                if (!hasPlayed[index] && countdownClips[index] != null)
                {
                    audioSource.PlayOneShot(countdownClips[index]);
                    hasPlayed[index] = true;
                }
            }
        }
        else if (isActive)
        {
            Color color = materialInstance.color;
            color.a = Mathf.MoveTowards(color.a, 0f, Time.deltaTime * 2f);
            materialInstance.color = color;

            for (int i = 0; i < hasPlayed.Length; i++)
                hasPlayed[i] = false;
        }
    }
}
