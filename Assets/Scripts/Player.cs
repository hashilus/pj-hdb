using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    PlayerID playerID;
    public PlayerID ID => playerID;

    public PlayerGun Gun { get; private set; }

    void Awake()
    {
        Gun = GetComponentInChildren<PlayerGun>();
    }
}