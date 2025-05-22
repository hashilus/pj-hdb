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
    
    private void Start()
    {
        controller.gameObject.SetActive(false);
    }

    void Update()
    {
        isLost = trackingBeforPos == trackingTransform.position && trackingBeforRot == trackingTransform.rotation;
        if (isLost)
        {
            Debug.Log("LOST");
            return;
        }

        trackingBeforPos = trackingTransform.position;
        trackingBeforRot = trackingTransform.rotation;
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            trackingOrgPos = trackingTransform.position;
            trackingOrgRot = trackingTransform.rotation;
            controller.gameObject.SetActive(true);
        }
        controller.position = controllerOrgTransform.position 
                            + controllerOrgTransform.TransformPoint(
                                PositionDiff() * Settings.Calibration.TrackerTransferCoefficient
                            )
                            + Settings.Calibration.ControllerOffset;
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
