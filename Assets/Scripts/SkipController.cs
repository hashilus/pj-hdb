using UnityEngine;

public class SkipController : MonoBehaviour
{
    void Update()
    {
        var isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        var isCtrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (Input.GetKey(KeyCode.F))
        {
            var timeScale = 2.0f;
            if (isShiftPressed) timeScale *= 2.0f;
            if (isCtrlPressed) timeScale *= 4.0f;
            Time.timeScale = timeScale;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }
}