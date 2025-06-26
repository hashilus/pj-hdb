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

        if (angle > Settings.System.ControllerLimitAngle)
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