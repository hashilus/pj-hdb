using UnityEngine;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public SettingsData settings;

    [Header("設定ファイル名 (StreamingAssets内)")]
    public string settingsFileName = "settings.json";

    private void Awake()
    {
        // Singletonパターン
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    public void LoadSettings()
    {
        string path = Path.Combine(Application.streamingAssetsPath, settingsFileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            settings = JsonUtility.FromJson<SettingsData>(json);

            Debug.Log($"Settings loaded: InitialTime={settings.initialTime}, FireLifeScale={settings.fireLifeScale}");
        }
        else
        {
            Debug.LogWarning($"Settings file not found: {path}");
            // デフォルト設定を作成
            settings = new SettingsData();
        }
    }
}
