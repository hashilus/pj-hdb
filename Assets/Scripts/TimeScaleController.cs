using UnityEngine;
using UnityEngine.Video;

public class TimeScaleController : MonoBehaviour
{
    [SerializeField]
    bool isCommandOn;
    
    [SerializeField]
    float upDownScale = 0.5f;
    
    void Update()
    {
        if (!isCommandOn)
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Time.timeScale += upDownScale;
            Debug.Log($"TimeScale : {Time.timeScale}");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Time.timeScale -= upDownScale;
            Debug.Log($"TimeScale : {Time.timeScale}");
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            Time.timeScale = 1f;
            Debug.Log($"TimeScale : {Time.timeScale}");
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            Time.timeScale = 10f;
            Debug.Log($"TimeScale : {Time.timeScale}");
        }
    }
}