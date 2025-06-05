using Hashilus.Setting;
using System.Text;
using UnityEngine;

public partial class Settings
{
    public const string FileName = "settings.xml";
    public const string GlobalSettingsPath = "Settings/" + FileName;
    public const string AppLocalSettingsPath = "AppLocalSettings/" + FileName;
    public const string UserLocalSettingsPath = "UserLocalSettings/" + FileName;

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

    static void LogSettingFileContent()
    {
        var sb = new StringBuilder();
        sb.AppendLine("設定情報");
        sb.AppendLine();

        var group = SettingsReaderWriter.GetFieldHierarchy<Settings>();
        WalkLogSettingGroup(group, 0, sb);

        Debug.Log(sb.ToString());
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
