using UnityEngine;

public class AirControllerBlowMonitor : MonoBehaviour
{
    [SerializeField]
    AirController airController;

    [SerializeField]
    GameObject targetObject;

    bool wasActive;

    void Start()
    {
        wasActive = targetObject.activeSelf;
    }

    void Update()
    {
        // テストコマンド
        if (Input.GetKeyDown(KeyCode.P))
        {
            targetObject.SetActive(!targetObject.activeSelf);
        }

        bool isActive = targetObject.activeSelf;
        
        // オブジェクトが非活性になった時
        if (wasActive && !isActive)
        {
            airController.StopBlow();
        }
        
        wasActive = isActive;
    }
}
