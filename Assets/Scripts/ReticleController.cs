using UnityEngine;

public class ReticleController : MonoBehaviour
{
    public Camera mainCamera;
    public float autoHideDelay = 1f;

    private Renderer rend;

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
            Debug.LogWarning("ReticleController: Renderer が見つかりません");

        if (mainCamera == null)
            mainCamera = Camera.main;

        targetPosition = transform.position;
    }

    void Update()
    {
        // カメラの向きに合わせる
        transform.forward = mainCamera.transform.forward;

        // スムーズに移動
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, Settings.System.ReticleMoveTime);

        // カメラからの距離に応じてスケールを調整
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);

        float targetScale = CalculateScaleForDistance(distance);
        transform.localScale = Vector3.one * targetScale;
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
    }
}
