using UnityEngine;

public class ControllerAngleChecker : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    private bool isLController;

    private bool isOutOfRange = false;

    private float originalXAngle;
    private float originalYAngle;

    private void Start()
    {
        if (targetTransform == null)
        {
            targetTransform = transform;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            originalXAngle = targetTransform.localEulerAngles.x;
            originalYAngle = targetTransform.localEulerAngles.y;
        }

        CheckAngles();
    }

    private void CheckAngles()
    {
        Vector3 localEulerAngles = targetTransform.localEulerAngles;
        float xAngle = localEulerAngles.x;
        float yAngle = localEulerAngles.y;

        // 角度を-180から180の範囲に正規化
        xAngle = NormalizeAngle(xAngle, originalXAngle);
        yAngle = NormalizeAngle(yAngle, originalYAngle);

        bool isXOutOfRange = xAngle < Settings.ControllerAngle.XAxisMinAngle || xAngle > Settings.ControllerAngle.XAxisMaxAngle;
        bool isYOutOfRange = yAngle < Settings.ControllerAngle.YAxisMinAngle || yAngle > Settings.ControllerAngle.YAxisMaxAngle;

        if (isXOutOfRange || isYOutOfRange)
        {
            if (!isOutOfRange)
            {
                Debug.Log($"Controller angle out of range - X: {xAngle:F1}°, Y: {yAngle:F1}°");
                isOutOfRange = true;
                AirBlowPermission.SetControllerAngleOutOfRange(
                    isLController ? AirBlowPermission.PlayerSelection.Player2 : AirBlowPermission.PlayerSelection.Player1,
                    true
                );
            }
        }
        else
        {
            if (isOutOfRange)
            {
                Debug.Log("Controller angle back in range");
                isOutOfRange = false;
                AirBlowPermission.SetControllerAngleOutOfRange(
                    isLController ? AirBlowPermission.PlayerSelection.Player2 : AirBlowPermission.PlayerSelection.Player1,
                    false
                );
            }
        }
    }

    private float NormalizeAngle(float angle, float originalAngle)
    {
        angle = angle - originalAngle;
        angle = angle % 360f;
        if (angle > 180f)
        {
            angle -= 360f;
        }
        return angle;
    }
} 