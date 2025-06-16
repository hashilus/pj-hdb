using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    static readonly HashSet<Player> players = new();
    public static IEnumerable<Player> Players => players;
    public static Player GetPlayer(PlayerID id) => Players.FirstOrDefault(p => p.ID == id);

    [SerializeField]
    PlayerID playerID;
    public PlayerID ID => playerID;

    public PlayerGun Gun { get; private set; }

    void Awake()
    {
        players.Add(this);

        Gun = GetComponentInChildren<PlayerGun>();
    }

    void OnDestroy()
    {
        players.Remove(this);
    }
}