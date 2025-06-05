#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace Hashilus.Setting
{
    public class SettingEnum<T> : SettingField<T>
    {
        public SettingEnum(string description, T defaultValue) :
            base(description + "(" + EnumHelper.GetNames<T>().JoinString(", ") + ")", defaultValue)
        { }

        protected override T Parse(string valueString)
        {
            return (T)Enum.Parse(typeof(T), valueString, true);
        }

        public override void OnGUI()
        {
#if UNITY_EDITOR
            SaveValueOnGUI((T)(object)EditorGUILayout.EnumPopup(Description, (Enum)(object)Value));
#endif
        }
    }
}