using System.Collections.Generic;
using UnityEngine;
using ChocDino.PartyIO;

/// <summary>
/// Mouse Party の入力を 1箇所で更新し、
/// 各デバイスのスクリーン位置とボタン状態を提供するブリッジ。
/// 自動生成され、シーンを跨いで常駐します。
/// </summary>
[DefaultExecutionOrder(-500)] // できるだけ早く Update させる
public class MousePartyInputBridge : MonoBehaviour
{
    const int MaxDevices = 4;

    static MousePartyInputBridge _inst;
    public  static MousePartyInputBridge Instance {
        get {
            if (_inst == null) {
                var go = new GameObject("MousePartyInputBridge");
                _inst = go.AddComponent<MousePartyInputBridge>();
                DontDestroyOnLoad(go);
            }
            return _inst;
        }
    }

    MouseManager _man;
    // 接続中デバイス一覧
    readonly List<Mouse> _mice = new List<Mouse>(MaxDevices);

    // 各デバイスのスクリーン座標（ピクセル）
    readonly Vector3[] _screenPos = new Vector3[MaxDevices];
    readonly bool[]   _hasInit    = new bool[MaxDevices];

    void OnEnable()
    {
        // サンプルと同じ順序：先にイベント登録→マネージャ生成
        MouseManager.ChangedConnectionState += OnConnChanged;
        _man = new MouseManager();
        Cursor.lockState = CursorLockMode.Locked; // サンプル準拠
    }

    void OnDisable()
    {
        MouseManager.ChangedConnectionState -= OnConnChanged;
        _man?.Dispose();
        _man = null;
        _mice.Clear();
        Cursor.lockState = CursorLockMode.None;
        for (int i = 0; i < MaxDevices; i++) _hasInit[i] = false;
    }

    void OnConnChanged(Mouse m)
    {
        // リストを取り直す（順番は MouseManager.All に合わせる）
        _mice.Clear();
        if (_man?.All != null) _mice.AddRange(_man.All);
    }

    void Update()
    {


        if (_man == null) return;
        _man.Update();

        var cam = Camera.main;
        if (cam == null) return;

        // 接続配列を最新化（生成直後やホットプラグ対応）
        _mice.Clear();
        if (_man.All != null) _mice.AddRange(_man.All);

        // 位置更新（サンプル準拠：スクリーン座標で積分）
        var min = Vector3.zero;
        var max = new Vector3(cam.pixelWidth, cam.pixelHeight, 0f);

        for (int i = 0; i < _mice.Count && i < MaxDevices; i++)
        {
            var mouse = _mice[i];

            if (!_hasInit[i]) {
                _screenPos[i] = new Vector3(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f, 0f);
                if (mouse.IsPositionAbsolute())
                    _screenPos[i] = mouse.PositionDelta; // 絶対座標デバイスなら初期値を合わせる
                _hasInit[i] = true;
            }

            var pos = _screenPos[i];
            var d   = mouse.PositionDelta;

            if (mouse.IsPositionAbsolute()) {
                pos = d; // 既にスクリーン座標
            } else if (d != Vector3.zero) {
                pos += new Vector3(d.x, -d.y, 0f); // Y 反転（Unity のスクリーン座標系）
            }

            // ゲームビューにクランプ
            pos = Vector3.Max(pos, min);
            pos = Vector3.Min(pos, max);

            _screenPos[i] = pos;

            Debug.Log($"{i}: Δ={_mice[i].PositionDelta} Abs={_mice[i].IsPositionAbsolute()}");

        }
    }

    // === 公開 API ===

    public static bool TryGetViewport(int deviceIndex, out Vector2 viewport01)
    {
        viewport01 = default;
        var inst = Instance;
        var cam  = Camera.main;
        if (inst == null || cam == null) return false;
        if (deviceIndex < 0 || deviceIndex >= inst._mice.Count) return false;
        var p = inst._screenPos[deviceIndex];
        viewport01 = new Vector2(
            cam.pixelWidth  <= 0 ? 0.5f : p.x / cam.pixelWidth,
            cam.pixelHeight <= 0 ? 0.5f : p.y / cam.pixelHeight
        );
        return true;
    }

    public static bool GetButton(int deviceIndex, MouseButton button)
    {
        var inst = Instance;
        if (inst == null) return false;
        if (deviceIndex < 0 || deviceIndex >= inst._mice.Count) return false;
        return inst._mice[deviceIndex].IsPressed(button);
    }

    public static int ConnectedCount
        => Instance?._mice.Count ?? 0;
}
