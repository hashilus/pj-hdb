// MousePartyHotkeys.cs
using UnityEngine;

public class MousePartyHotkeys : MonoBehaviour
{
    [SerializeField] MousePartyInputRouter router;

    [Header("Hotkeys")]
    [SerializeField] KeyCode swapKey   = KeyCode.Tab;   // P1<->P2 “ü‘Ö
    [SerializeField] KeyCode rebindKey = KeyCode.R;     // Ž©“®ƒoƒCƒ“ƒh‚â‚è’¼‚µ

    void Update()
    {
        if (router == null) return;

        if (Input.GetKeyDown(swapKey))
        {
            router.SwapPlayers();
        }
        if (Input.GetKeyDown(rebindKey))
        {
            router.Rebind();
        }
    }
}
