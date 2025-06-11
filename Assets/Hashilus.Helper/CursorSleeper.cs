using UnityEngine;
using UnityEngine.InputSystem;

namespace Hashilus.CursorSleeper
{
    public class CursorSleeper : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Initialize()
        {
            var cursorSleeper = new GameObject(nameof(CursorSleeper)).AddComponent<CursorSleeper>();
            DontDestroyOnLoad(cursorSleeper.gameObject);
        }

        const float ElapsedSecondsNeededForSleep = 2f;
        float idleSeconds;
        Vector3 previousMousePosition;

        void Update()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var currentMousePosition = new Vector3(mousePosition.x, mousePosition.y, 0);
            if (currentMousePosition == previousMousePosition)
            {
                idleSeconds += Time.deltaTime;
            }
            else
            {
                idleSeconds = 0;
            }

            var sleeping = idleSeconds > ElapsedSecondsNeededForSleep;
            Cursor.visible = !sleeping;

            previousMousePosition = currentMousePosition;
        }
    }
}
