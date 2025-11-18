using System;
using System.Collections.Generic;
using UnityEngine;
using ChocDino.PartyIO;

/// <summary>
/// MouseParty Lite想定のルーター：
/// - Sinden LightGun を最優先で割当（VID_16C0 & PID_0F39）
/// - Sindenが2丁: P1/P2 に割当
/// - Sindenが1丁: P1=Sinden, P2=null（マウスは無視）
/// - Sindenが0:   P1=最初に見えたマウス, P2=null（シングル運用）
/// </summary>
public class MousePartyInputRouter : MonoBehaviour
{
    private MouseManager _mouseMan;
    private readonly List<Mouse> _mice = new List<Mouse>(8); // Liteは内部で最大2までだが念のため

    [Header("Players")]
    [SerializeField] int playersNeeded = 2; // 2スロット固定（P1/P2）

    // 現在の割り当て（[0]=P1, [1]=P2）
    private Mouse[] _assigned;

    // ログ出力のオン/オフ
    [Header("Debug")]
    [SerializeField] bool verboseLog = true;

    void OnEnable()
    {
        MouseManager.ChangedConnectionState += OnChangedMouseConnectionState;
        _mouseMan = new MouseManager();
        _assigned = new Mouse[playersNeeded];

        // 起動直後の列挙は OnChangedMouseConnectionState が来ないこともあるので、
        // 1フレーム後に強制スキャンして再計算
        Invoke(nameof(RecomputeAssignments), 0.0f);
    }

    void OnDisable()
    {
        MouseManager.ChangedConnectionState -= OnChangedMouseConnectionState;
        _mouseMan?.Dispose();
        _mouseMan = null;
        _mice.Clear();
        _assigned = null;
    }

    private void Start()
    {
        //マウスカーソル非表示
        Cursor.visible = false;
    }


    void Update()
    {
        _mouseMan?.Update();
        // ここでは再計算しない（接続/切断/手動Rebind時のみ再計算）
    }

    // ===== 接続・切断検知 =====
    void OnChangedMouseConnectionState(Mouse mouse)
    {
        if (mouse.ConnectionState == MouseConnectionState.Connected)
        {
            if (!_mice.Contains(mouse)) _mice.Add(mouse);
            if (verboseLog) Debug.Log($"[MouseParty] Connected: {Describe(mouse)}");
        }
        else
        {
            _mice.Remove(mouse);
            // assignedからも除去
            if (_assigned != null)
            {
                for (int i = 0; i < _assigned.Length; i++)
                    if (_assigned[i] == mouse) _assigned[i] = null;
            }
            if (verboseLog) Debug.Log($"[MouseParty] Disconnected: {Describe(mouse)}");
        }

        RecomputeAssignments();
    }

    // ===== 割当ロジック（要件どおり） =====
    void RecomputeAssignments()
    {
        if (_assigned == null) return;

        // デバイスを現在の見つかった順でコピー（安定した順序のため）
        var snapshot = new List<Mouse>(_mice);

        var sindens = new List<Mouse>(2);
        var others  = new List<Mouse>(2);

        foreach (var m in snapshot)
        {
            if (m == null) continue;
            if (IsLightGun(m)) sindens.Add(m);
            else others.Add(m);
        }

        // クリア
        for (int i = 0; i < _assigned.Length; i++) _assigned[i] = null;

        if (sindens.Count >= 2)
        {
            _assigned[0] = sindens[0];
            _assigned[1] = sindens[1];
        }
        else if (sindens.Count == 1)
        {
            _assigned[0] = sindens[0];
            // P2はnullのまま（マウスは完全無視）
        }
        else
        {
            // Sindenが1つも無い場合のみ、マウスをP1に入れてシングル運用
            _assigned[0] = (others.Count > 0) ? others[0] : null;
            // P2はnull
        }

        if (verboseLog)
        {
            Debug.Log($"[MouseParty] Assigned -> [P1:{Describe(_assigned,0)} , P2:{Describe(_assigned,1)}]");
        }
    }

    // ===== 公開API =====

    /// <summary>P1=0, P2=1 のデバイスを返す（未割当は null）</summary>
    public Mouse GetMouseForIndex(int index)
    {
        if (_assigned == null || index < 0 || index >= _assigned.Length) return null;
        return _assigned[index];
    }

    public int ConnectedCount => _mice.Count;

    /// <summary>手動で再スキャン・再割当（メニューやホットキーから）</summary>
    public void Rebind()
    {
        RecomputeAssignments();
        if (verboseLog) Debug.Log("[MouseParty] Rebind triggered");
    }

    /// <summary>P1↔P2の入替（両方がSinden時のみ意味がある）</summary>
    public void SwapPlayers()
    {
        if (_assigned == null || _assigned.Length < 2) return;
        var tmp = _assigned[0];
        _assigned[0] = _assigned[1];
        _assigned[1] = tmp;

        if (verboseLog)
            Debug.Log($"[MouseParty] Swapped -> [P1:{Describe(_assigned,0)} , P2:{Describe(_assigned,1)}]");
    }

    /// <summary>UI表示用の状態テキスト</summary>
    public string GetStatusText()
    {
        string SlotName(Mouse m)
        {
            if (m == null) return "(未割当)";
            string tag = IsLightGun(m) ? "[Sinden]" : "[Mouse]";
            return $"{tag} {Describe(m)}";
        }
        var p1 = (_assigned != null && _assigned.Length > 0) ? SlotName(_assigned[0]) : "(未割当)";
        var p2 = (_assigned != null && _assigned.Length > 1) ? SlotName(_assigned[1]) : "(未割当)";
        return $"P1: {p1}\nP2: {p2}";
    }

    // ===== ヘルパ =====

    // InstanceIdのVID/PIDでSinden判定。無い環境向けに絶対座標も保険で見る。
    bool IsLightGun(Mouse m)
    {
        // InstanceId（例: "HID\\VID_16C0&PID_0F39&MI_02\\..."}
        string id = TryGetProperty<string>(m, "InstanceId");
        if (!string.IsNullOrEmpty(id))
        {
            string lid = id.ToLowerInvariant();
            if (lid.Contains("vid_16c0") && lid.Contains("pid_0f39"))
                return true; // Sinden LightGun
        }

        // 保険：絶対座標デバイスはライトガン寄り
        try
        {
            if (m.IsPositionAbsolute()) return true;
        }
        catch { /* 実装差異で例外なら無視 */ }

        return false;
    }

    T TryGetProperty<T>(object obj, string prop)
    {
        try
        {
            var p = obj.GetType().GetProperty(prop,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (p != null && typeof(T).IsAssignableFrom(p.PropertyType))
                return (T)p.GetValue(obj);
        }
        catch { }
        return default;
    }

    string Describe(Mouse m)
    {
        if (m == null) return "null";
        string name = TryGetProperty<string>(m, "FriendlyName");
        if (string.IsNullOrEmpty(name))
            name = TryGetProperty<string>(m, "ManufacturerName");
        if (string.IsNullOrEmpty(name))
            name = TryGetProperty<string>(m, "DeviceId")?.ToString();
        if (string.IsNullOrEmpty(name))
            name = TryGetProperty<string>(m, "InstanceId");
        if (string.IsNullOrEmpty(name))
            name = m.ToString();
        return name;
    }
    string Describe(Mouse[] arr, int idx) => (arr == null || idx < 0 || idx >= arr.Length) ? "null" : Describe(arr[idx]);
}
