using Hashilus.Setting;
using System.IO;
using System.Text;
using UnityEngine;

public partial class Settings
{
    public const string FileName = "settings.xml";

    const string globalSettingsPath = "Settings/" + FileName;
    const string appLocalSettingsPath = "AppLocalSettings/" + FileName;
    const string userLocalSettingsPath = "UserLocalSettings/" + FileName;

#if UNITY_ANDROID && !UNITY_EDITOR
    public static string GlobalSettingsPath => Path.Combine(Application.persistentDataPath, globalSettingsPath);
    public static string AppLocalSettingsPath => Path.Combine(Application.persistentDataPath, appLocalSettingsPath);
    public static string UserLocalSettingsPath => Path.Combine(Application.persistentDataPath, userLocalSettingsPath);
#elif UNITY_IOS && !UNITY_EDITOR
    public static string GlobalSettingsPath => Path.Combine(Application.persistentDataPath, globalSettingsPath);
    public static string AppLocalSettingsPath => Path.Combine(Application.persistentDataPath, appLocalSettingsPath);
    public static string UserLocalSettingsPath => Path.Combine(Application.persistentDataPath, userLocalSettingsPath);
#else
    public static string GlobalSettingsPath => globalSettingsPath;
    public static string AppLocalSettingsPath => appLocalSettingsPath;
    public static string UserLocalSettingsPath => userLocalSettingsPath;
#endif

#if UNITY_EDITOR
    public static bool editorLoadFinished;
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        Load();
        LogSettingFileContent();
    }

    public static void Load()
    {
        SettingsReaderWriter.Load<Settings>(GlobalSettingsPath);
        SettingsReaderWriter.Load<Settings>(AppLocalSettingsPath, allowNoFile: true);
        SettingsReaderWriter.Load<Settings>(UserLocalSettingsPath, allowNoFile: true);
    }

    public static void Save()
    {
        SettingsReaderWriter.Save<Settings>(GlobalSettingsPath);
    }

    public static void SaveAsUserLocal()
    {
        SettingsReaderWriter.Save<Settings>(UserLocalSettingsPath, saveModifiedOnly: true);
    }

    public static void Remove()
    {
        SettingsReaderWriter.Remove<Settings>(GlobalSettingsPath);
    }

    public static void RemoveUserLocal()
    {
        SettingsReaderWriter.Remove<Settings>(UserLocalSettingsPath);
    }

    static void LogSettingFileContent()
    {
        var sb = new StringBuilder();
        sb.AppendLine("設定情報");
        sb.AppendLine();

        var group = SettingsReaderWriter.GetFieldHierarchy<Settings>();
        WalkLogSettingGroup(group, 0, sb);

        UnityEngine.Debug.Log(sb.ToString());
    }

    static void WalkLogSettingGroup(SettingsReaderWriter.SettingFieldGroup group, int indent, StringBuilder sb)
    {
        var spaces = new string(' ', indent * 2);
        sb.AppendLine(spaces + "+ " + (group.name ?? "Settings ") + " ----------");

        var fieldSpaces = new string(' ', (indent + 1) * 2);
        foreach (var field in group.fields)
        {
            sb.AppendLine(fieldSpaces + "[" + field.Key + "] " + field.ToString());
        }

        foreach (var childGroup in group.childGroups)
        {
            WalkLogSettingGroup(childGroup, indent + 1, sb);
        }
    }

    public static void Reset()
    {
        WalkReset(SettingsReaderWriter.GetFieldHierarchy<Settings>());
    }

    static void WalkReset(SettingsReaderWriter.SettingFieldGroup group)
    {
        foreach (var field in group.fields)
        {
            field.Reset();
        }

        foreach (var childGroup in group.childGroups)
        {
            WalkReset(childGroup);
        }
    }

}
