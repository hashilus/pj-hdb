#if UNITY_EDITOR
using Hashilus.Setting;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildScript
{
    static readonly string DistDirBase = "./Builds/";
    static string GetDistDir() => Path.Combine(DistDirBase, $"{Application.productName}_{Application.version}");

    [MenuItem("Hashilus/Build Release", priority = 10000)]
    public static void Build()
    {
        BuildStandalone();
        ExportSettings();
    }

    static void BuildStandalone()
    {
        var buildPlayerOptions = new BuildPlayerOptions()
        {
            scenes = new[] { "Assets/Main.unity" },
            locationPathName = Path.Combine(GetDistDir(), $"{Application.productName}.exe"),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None,
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    static void ExportSettings()
    {
        Settings.Reset();
        Settings.RemoveUserLocal();
        Settings.Save();
        Settings.Load();

        Settings.System.IsUseTracker.Value = true;

        var settingsDir = Path.Combine(GetDistDir(), "Settings");
        var settingsFile = new FileInfo(Path.Combine(settingsDir, "settings.xml"));
        if (Directory.Exists(settingsDir))
        {
            Directory.Delete(settingsDir, true);
        }
        SettingsReaderWriter.Save<Settings>(settingsFile.FullName);

        Settings.Reset();
        Settings.Load();
    }
}
#endif
