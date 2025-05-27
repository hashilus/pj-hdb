using UnityEngine;

public static class AirBlowPermission
{
    [System.Flags]
    public enum PlayerSelection
    {
        Player1,
        Player2,
        Both
    }

    private static bool[] playerPermissions;

    static AirBlowPermission()
    {
        // プレイヤー数に応じて配列を初期化（2プレイヤー）
        playerPermissions = new bool[2];
    }

    /// <summary>
    /// プレイヤー選択状態を設定
    /// </summary>
    public static void SetPlayerSelection(PlayerSelection selection, bool isEnable)
    {
        if(selection == PlayerSelection.Both)
        {
            playerPermissions[0] = isEnable;
            playerPermissions[1] = isEnable;
        }
        else if(selection == PlayerSelection.Player1)
        {
            playerPermissions[0] = isEnable;
        }
        else if(selection == PlayerSelection.Player2)
        {
            playerPermissions[1] = isEnable;
        }
        Debug.Log($"Player selection set to: {selection}");
    }

    /// <summary>
    /// 指定したプレイヤーの噴射許可状態を取得
    /// </summary>
    /// <param name="player">プレイヤー選択</param>
    /// <returns>噴射が許可されている場合はtrue</returns>
    public static bool CanBlow(PlayerSelection player)
    {
        if (player == PlayerSelection.Both)
        {
            return playerPermissions[0] && playerPermissions[1];
        }

        if (player == PlayerSelection.Player1)
        {
            return playerPermissions[0];
        }

        if (player == PlayerSelection.Player2)
        {
            return playerPermissions[1];
        }

        return false;
    }

    /// <summary>
    /// すべてのプレイヤーの噴射を禁止
    /// </summary>
    public static void DisableAllPermissions()
    {
        SetPlayerSelection(PlayerSelection.Both,false);
    }
} 