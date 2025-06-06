﻿#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hashilus.Setting
{
    public class SettingString : SettingField<string>
    {
        public SettingString(string description, string defaultValue) : base(description, defaultValue) { }

        public static implicit operator string(SettingString v)
        {
            return v.Value;
        }

        protected override string Parse(string valueString)
        {
            return valueString;
        }

        public override void OnGUI()
        {
#if UNITY_EDITOR
            SaveValueOnGUI(EditorGUILayout.TextField(Description, Value));
#endif
        }
    }
}