using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibratedPositionProvider : MonoBehaviour
{
    [SerializeField]
    Transform trackingTransform;
    
    [SerializeField]
    Transform controllerOrgTransform;
    
    [SerializeField]
    Transform controller;

    Vector3 trackingOrgPos;
    Quaternion trackingOrgRot;

    Vector3 trackingBeforPos;
    Quaternion trackingBeforRot;
    bool isLost;
    
    void Start()
    {
        // Debug.Log(Camera.main.gameObject.name);

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
        }
        controller.position = controllerOrgTransform.TransformPoint(
                                PositionDiff() * Settings.Calibration.TrackerTransferCoefficient
                            )
                            + Settings.Calibration.ControllerOffset;

        // Debug.Log("controllerOrgTransform.name : " + controllerOrgTransform.gameObject.name);
        // Debug.Log("controllerOrgTransform.position : " + controllerOrgTransform.position);
        // Debug.Log("controllerOrgTransform.TransformPoint(PositionDiff() * Settings.Calibration.TrackerTransferCoefficient) : " + controllerOrgTransform.TransformPoint(PositionDiff() * Settings.Calibration.TrackerTransferCoefficient));
        // Debug.Log("Settings.Calibration.ControllerOffset : " + Settings.Calibration.ControllerOffset);
        // Debug.Log("controller.position : " + controller.position);
        // Debug.Log("--------------------------------");


        controller.rotation = controllerOrgTransform.rotation * RotationDiff();
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
