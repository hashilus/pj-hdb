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
    private static bool[] controllerAngleOutOfRange;

    static AirBlowPermission()
    {
        // プレイヤー数に応じて配列を初期化（2プレイヤー）
        playerPermissions = new bool[2];
        controllerAngleOutOfRange = new []{false,false};
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
    /// コントローラーの角度範囲外状態を設定
    /// </summary>
    public static void SetControllerAngleOutOfRange(PlayerSelection player, bool isOutOfRange)
    {
        // 未完成の為一時的に不使用にする（falseのままにする
        
        //
        // if(player == PlayerSelection.Both)
        // {
        //     controllerAngleOutOfRange[0] = isOutOfRange;
        //     controllerAngleOutOfRange[1] = isOutOfRange;
        // }
        // else if(player == PlayerSelection.Player1)
        // {
        //     controllerAngleOutOfRange[0] = isOutOfRange;
        // }
        // else if(player == PlayerSelection.Player2)
        // {
        //     controllerAngleOutOfRange[1] = isOutOfRange;
        // }
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
            if(controllerAngleOutOfRange[0] || controllerAngleOutOfRange[1])
            {
                return false;
            }
            
            return playerPermissions[0] && playerPermissions[1];
        }

        if (player == PlayerSelection.Player1)
        {
            if(controllerAngleOutOfRange[0])
            {
                return false;
            }

            return playerPermissions[0];
        }

        if (player == PlayerSelection.Player2)
        {
            if(controllerAngleOutOfRange[1])
            {
                return false;
            }
            
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