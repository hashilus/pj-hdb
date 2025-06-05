using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    PlayerID playerID;
    public PlayerID ID => playerID;

    public PlayerBarrel Barrel { get; private set; }

    void Awake()
    {
        Barrel = GetComponentInChildren<PlayerBarrel>();
    }
}