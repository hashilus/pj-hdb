using UnityEngine;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public SettingsData settings;

    [Header("�ݒ�t�@�C���� (StreamingAssets��)")]
    public string settingsFileName = "settings.json";

    private void Awake()
    {
        // Singleton�p�^�[��
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
            // �f�t�H���g�ݒ���쐬
            settings = new SettingsData();
        }
    }
}
