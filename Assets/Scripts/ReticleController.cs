using UnityEngine;

public class ReticleController : MonoBehaviour
{
    public Camera mainCamera;
    public float autoHideDelay = 1f;
    public float moveSmoothTime = 0.1f; // �X���[�Y�ړ����x

    private float hideTimer = -1f;
    private bool isVisible = false;

    private Renderer rend;

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
            Debug.LogWarning("ReticleController: Renderer ��������܂���");

        if (mainCamera == null)
            mainCamera = Camera.main;

        SetVisible(false);
        targetPosition = transform.position;
    }

    void Update()
    {
        if (isVisible)
        {
            // �J��������������
            transform.forward = mainCamera.transform.forward;

            // �Ȃ߂炩�Ɉړ�
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, moveSmoothTime);

            // ��\���^�C�~���O
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
