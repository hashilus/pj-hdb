#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace Hashilus.Setting
{
    public class SettingTimeSpan : SettingField<TimeSpan>
    {
        public float TotalSeconds => (float)Value.TotalSeconds;

        public SettingTimeSpan(string description, TimeSpan defaultValue) : base(description + "(分:秒 / 秒)", defaultValue) { }

        protected override TimeSpan Parse(string valueString)
        {
            var colonIndex = valueString.IndexOf(':');
            if (colonIndex < 0)
            {
                return TimeSpan.FromSeconds(float.Parse(valueString));
            }
            else
            {
                var minutes = float.Parse(valueString.Substring(0, colonIndex));
                var seconds = float.Parse(valueString.Substring(colonIndex + 1));
                return TimeSpan.FromSeconds(minutes * 60 + seconds);
            }
        }

        public override void OnGUI()
        {
#if UNITY_EDITOR
            var inputValue = EditorGUILayout.TextField(Description, ToString());

            try
            {
                SaveValueOnGUI(Parse(inputValue));
            }
            catch
            {
                //L.Warn(LogPlace.Setting, $"{Key}の値{inputValue}が{typeof(TimeSpan).Name}として読み込めませんでした。");
            }
#endif
        }

        public override string ToString()
        {
            if (Value.TotalMinutes >= 1f)
            {
                return $"{((int)Value.TotalMinutes).ToString()}:{Value.Seconds:00}";
            }
            else
            {
                return Value.TotalSeconds.ToString("0.00");
            }
        }
    }
}
