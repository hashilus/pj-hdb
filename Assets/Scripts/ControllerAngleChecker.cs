using System;
using UnityEngine;

public class ControllerAngleChecker : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    private bool isLController;

    private bool isOutOfRange = false;

    CalibrationPositionHolder.PlayerSelection playerSelection => isLController
        ? CalibrationPositionHolder.PlayerSelection.Player2
        : CalibrationPositionHolder.PlayerSelection.Player1;

    private void Start()
    {
        if (targetTransform == null)
        {
            targetTransform = transform;
        }
    }

    private void Update()
    {
        CheckAngles();
    }

    private void CheckAngles()
    {
        var angle = Quaternion.Angle(CalibrationPositionHolder.Instance.GetRotation(playerSelection),
                                     targetTransform.localRotation);

        var v = (targetTransform.localRotation * Quaternion.Inverse(CalibrationPositionHolder.Instance.GetRotation(playerSelection))).eulerAngles;
        var x = v.x > 180f ? Mathf.Abs(v.x - 360f) : v.x;
        var y = v.y > 180f ? Mathf.Abs(v.y - 360f) : v.y;
        var z = v.z > 180f ? Mathf.Abs(v.z - 360f) : v.z;
        
        if (x > Settings.System.ControllerLimitXAngle || y > Settings.System.ControllerLimitYAngle)
        {
            if (!isOutOfRange)
            {
                Debug.Log($"Controller angle out of range - angle: {angle:F1}°");
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
} 