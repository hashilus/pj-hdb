using UnityEngine;

public class ReticleController : MonoBehaviour
{
    public Camera mainCamera;
    public float autoHideDelay = 1f;
    public float moveSmoothTime = 0.1f; // スムーズ移動速度

    private float hideTimer = -1f;
    private bool isVisible = false;

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

        SetVisible(false);
        targetPosition = transform.position;
    }

    void Update()
    {
        if (isVisible)
        {
            // カメラ方向を向く
            transform.forward = mainCamera.transform.forward;

            // なめらかに移動
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, moveSmoothTime);

            // 非表示タイミング
            if (Time.time > hideTimer)
                SetVisible(false);
        }
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
