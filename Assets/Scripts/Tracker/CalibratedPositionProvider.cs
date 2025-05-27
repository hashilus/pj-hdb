using System;
using System.Collections;
using UnityEngine;

public class CalibratedPositionProvider : MonoBehaviour
{
    [SerializeField]
    Transform trackingTransform;
    
    [SerializeField]
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

    Vector3 ControllerOffset => isLController ? Settings.Calibration.ControllerOffsetL : Settings.Calibration.ControllerOffsetR;
    
    void Start()
    {
        controllerOrgTransform = Camera.main.transform;

        controller.gameObject.SetActive(false);

        if (!Settings.System.IsUseTracker)
        {
            gameObject.SetActive(false);
        }

        // 遅延させないと値が拾えない
        StartCoroutine(Delay(() => trackingTransform.gameObject.SetActive(true), 1f));
    }

    IEnumerator Delay(Action a, float t) { yield return new WaitForSeconds(t); a(); }

    void Update()
    {
        // isLost = trackingBeforPos == trackingTransform.position && trackingBeforRot == trackingTransform.rotation;
        // if (isLost)
        // {
        //     Debug.Log("LOST");
        //     return;
        // }

        trackingBeforPos = trackingTransform.position;
        trackingBeforRot = trackingTransform.rotation;
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            trackingOrgPos = trackingTransform.position;
            trackingOrgRot = trackingTransform.rotation;
            controller.gameObject.SetActive(true);

            AirBlowPermission.SetPlayerSelection(
                isLController ? AirBlowPermission.PlayerSelection.Player2 : AirBlowPermission.PlayerSelection.Player1,
                true
            );
        }

        controller.rotation = controllerOrgTransform.rotation * RotationDiff();

        controller.position = controllerOrgTransform.TransformPoint(
                                PositionDiff() * Settings.Calibration.TrackerTransferCoefficient 
                                + ControllerOffset
                            );

        // Debug.Log("controllerOrgTransform.name : " + controllerOrgTransform.gameObject.name);
        // Debug.Log("controllerOrgTransform.position : " + controllerOrgTransform.position);
        // Debug.Log("controllerOrgTransform.TransformPoint(PositionDiff() * Settings.Calibration.TrackerTransferCoefficient) : " + controllerOrgTransform.TransformPoint(PositionDiff() * Settings.Calibration.TrackerTransferCoefficient));
        // Debug.Log("Settings.Calibration.ControllerOffset : " + Settings.Calibration.ControllerOffset);
        // Debug.Log("controller.position : " + controller.position);
        // Debug.Log("--------------------------------");

    }

    Vector3 PositionDiff()
    {
        var diff = trackingTransform.position - trackingOrgPos;

        // X軸とZ軸を反転させる
        return new Vector3(-diff.x, diff.y, -diff.z);
    }

    Quaternion RotationDiff()
    {
        Quaternion diff = Quaternion.Inverse(trackingTransform.rotation * Quaternion.Inverse(trackingOrgRot));
        Vector3 eulerAngles = diff.eulerAngles;
        // Y軸回転のみを反転させる
        return Quaternion.Euler(eulerAngles.x, -eulerAngles.y, eulerAngles.z);
    }
}
