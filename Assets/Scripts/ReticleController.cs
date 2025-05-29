using UnityEngine;

public class ReticleController : MonoBehaviour
{
    public Camera mainCamera;
    public float autoHideDelay = 1f;
    public float moveSmoothTime = 0.1f; // スムーズ移動速度
    public float scaleSmoothTime = 0.1f; // スケール変更のスムーズ時間

    private float hideTimer = -1f;
    private bool isVisible = false;

    private Renderer rend;
    private Vector3 originalScale;

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;
    private float scaleVelocity = 0f;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
            Debug.LogWarning("ReticleController: Renderer が見つかりません");

        if (mainCamera == null)
            mainCamera = Camera.main;

        originalScale = transform.localScale;
        SetVisible(false);
        targetPosition = transform.position;
    }

    void Update()
    {
        if (isVisible)
        {
            // カメラの向きに合わせる
            transform.forward = mainCamera.transform.forward;

            // スムーズに移動
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, moveSmoothTime);

            // カメラからの距離に応じてスケールを調整
            float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
            float targetScale = CalculateScaleForDistance(distance);
            float currentScale = transform.localScale.x;
            float newScale = Mathf.SmoothDamp(currentScale, targetScale, ref scaleVelocity, scaleSmoothTime);
            transform.localScale = Vector3.one * newScale;

            // 表示タイマー
            if (Time.time > hideTimer)
                SetVisible(false);
        }
    }

    private float CalculateScaleForDistance(float distance)
    {
        // カメラの視野角と距離から、画面上での目標サイズを維持するためのスケールを計算
        float screenHeight = 2.0f * distance * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float targetWorldSize = screenHeight * Settings.System.TargetScreenSize;
        return targetWorldSize;
    }

    public void ShowAt(Vector3 worldPosition)
    {
        targetPosition = worldPosition;
        SetVisible(true);
        hideTimer = Time.time + autoHideDelay;
    }

    private void SetVisible(bool visible)
    {
        isVisible = visible;
        if (rend != null)
            rend.enabled = visible;
    }
}
