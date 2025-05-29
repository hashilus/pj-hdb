using System;
using System.Collections;
using UnityEngine;

public class CalibratedPositionProvider : MonoBehaviour
{
    [SerializeField]
    Transform trackingTransform;
    
    Transform controllerOrgTransform;
    
    [SerializeField]
    Transform controller;

    [SerializeField]
    bool isLController;
    
    Vector3 trackingOrgPos;
    Quaternion trackingOrgRot;

    Vector3 trackingBeforPos;
    Quaternion trackingBeforRot;

    bool isLost;
    float lostCheckTime;

    Vector3 ControllerOffset => isLController ? Settings.Calibration.ControllerOffsetL : Settings.Calibration.ControllerOffsetR;
    
    CalibrationPositionHolder.PlayerSelection playerSelection => isLController
        ? CalibrationPositionHolder.PlayerSelection.Player2
        : CalibrationPositionHolder.PlayerSelection.Player1;

    void Start()
    {
        controllerOrgTransform = Camera.main.transform;

        controller.gameObject.SetActive(false);

        if (!Settings.System.IsUseTracker)
        {
            gameObject.SetActive(false);
        }

        if(CalibrationPositionHolder.Instance.IsCalibrated)
        {
            trackingOrgPos = CalibrationPositionHolder.Instance.GetPosition(playerSelection);
            trackingOrgRot = CalibrationPositionHolder.Instance.GetRotation(playerSelection);
            controller.gameObject.SetActive(true);
        }

        // 遅延させないと値が拾えない
        StartCoroutine(Delay(() => trackingTransform.gameObject.SetActive(true), 1f));
    }

    IEnumerator Delay(Action a, float t) { yield return new WaitForSeconds(t); a(); }

    void Update()
    {
        LostCheck();
        
        controller.rotation = controllerOrgTransform.rotation * RotationDiff();
        controller.position = controllerOrgTransform.TransformPoint(
                                PositionDiff() * Settings.Calibration.TrackerTransferCoefficient 
                                + ControllerOffset
                            );
    }

    void LostCheck()
    {
        if (trackingBeforPos == trackingTransform.position && trackingBeforRot == trackingTransform.rotation)
        {
            lostCheckTime += Time.deltaTime;
            if (lostCheckTime >= Settings.System.TrackerLostCheckDuration)
            {
                if(!isLost) Debug.Log("LOST");
                isLost = true;
            }
        }
        else
        {
            lostCheckTime = 0f;
            if(isLost) Debug.Log("LOST reset");
            isLost = false;
        }

        trackingBeforPos = trackingTransform.position;
        trackingBeforRot = trackingTransform.rotation;
    }

    Vector3 PositionDiff()
    {
        var diff = trackingTransform.position - trackingOrgPos;

        // 座標変換（X軸とZ軸を反転させる）
        return new Vector3(-diff.x, diff.y, -diff.z);
    }

    Quaternion RotationDiff()
    {
        Quaternion diff = Quaternion.Inverse(trackingTransform.rotation * Quaternion.Inverse(trackingOrgRot));
        Vector3 eulerAngles = diff.eulerAngles;
        return Quaternion.Euler(
            eulerAngles.x+Settings.Calibration.ControllerXRotationOffset, 
            -eulerAngles.y, // 座標変換（Y軸回転を反転させる）
            eulerAngles.z);
    }
    
    public void Calibrate()
    {
        trackingOrgPos = trackingTransform.position;
        trackingOrgRot = trackingTransform.rotation;
        controller.gameObject.SetActive(true);

        CalibrationPositionHolder.Instance.SetPosition(playerSelection, trackingOrgPos);
        CalibrationPositionHolder.Instance.SetRotation(playerSelection, trackingOrgRot);
        CalibrationPositionHolder.Instance.IsCalibrated = true;

        AirBlowPermission.SetPlayerSelection(
            isLController ? AirBlowPermission.PlayerSelection.Player2 : AirBlowPermission.PlayerSelection.Player1,
            true
        );
    }
}
