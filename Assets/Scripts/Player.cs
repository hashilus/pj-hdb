using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    PlayerType playerType;
    public PlayerType Type => playerType;

    public PlayerBarrel Barrel { get; private set; }

    void Awake()
    {
        Barrel = GetComponentInChildren<PlayerBarrel>();
    }
}