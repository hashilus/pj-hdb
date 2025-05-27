using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationUiController : MonoBehaviour
{
    [SerializeField]
    GameObject Ui;

    void Start()
    {
        Ui.SetActive(false);
    }

    void Update()
    {
        if(!Ui.activeSelf)
        {
            if(Input.GetKeyDown(KeyCode.C))
            {
                Ui.SetActive(true);
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.C))
            {
                foreach (var provider in Object.FindObjectsOfType<CalibratedPositionProvider>())
                {
                    if (provider != null)
                    {
                        provider.Calibrate();
                    }
                }
                Ui.SetActive(false);
            }
            else if(Input.GetKeyDown(KeyCode.Backspace))
            {
                Ui.SetActive(false);
            }
        }
    }
}
