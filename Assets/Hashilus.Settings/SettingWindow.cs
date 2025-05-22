#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hashilus.Setting
{
    public class SettingWindow : EditorWindow
    {
        readonly Dictionary<SettingsReaderWriter.SettingFieldGroup, bool> foldoutDict = new Dictionary<SettingsReaderWriter.SettingFieldGroup, bool>();
        Vector2 scrollPosition;

        [MenuItem("Hashilus/Setting", priority = 1)]
        static void Init()
        {
            var window = (SettingWindow)GetWindow(typeof(SettingWindow));
            window.Show();
        }

        void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                ShowNotification(new GUIContent("コンパイル中は待ってね"));
                return;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            if (!Settings.editorLoadFinished)
            {
                Settings.editorLoadFinished = true;
                Settings.Load();
            }

            EditorGUIUtility.labelWidth = position.width * 0.6f;

            var group = SettingsReaderWriter.GetFieldHierarchy<Settings>();
            WalkGroupOnGUI(group, "Settings");

            if (GUILayout.Button("デフォルト設定で書き出し（テンプレート生成用）"))
            {
                Settings.Reset();
                Settings.Save();
            }

            GUILayout.EndScrollView();
        }

        void WalkGroupOnGUI(SettingsReaderWriter.SettingFieldGroup group, string rootName)
        {
            bool f;
            if (!foldoutDict.TryGetValue(group, out f))
            {
                f = true;
                foldoutDict.Add(group, f);
            }

            foldoutDict[group] = EditorGUILayout.Foldout(foldoutDict[group], group.name ?? rootName);
            if (foldoutDict[group])
            {
                EditorGUI.indentLevel += 1;
                foreach (var field in group.fields)
                {
                    field.OnGUI();
                }

                foreach (var childGroup in group.childGroups)
                {
                    WalkGroupOnGUI(childGroup, rootName);
                }
                EditorGUI.indentLevel -= 1;
            }
        }
    }
}
#endif
