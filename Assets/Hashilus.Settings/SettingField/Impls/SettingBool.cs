#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hashilus.Setting
{
    public class SettingBool : SettingField<bool>
    {
        public SettingBool(string description, bool defaultValue) : base(description, defaultValue) { }

        protected override bool Parse(string valueString)
        {
            return bool.Parse(valueString);
        }

        public override void OnGUI()
        {
#if UNITY_EDITOR
            Value = EditorGUILayout.Toggle(Description, Value);
            if (!Value.Equals(previousValue))
            {
                Settings.SaveAsUserLocal();
                previousValue = Value;
            }
#endif
        }
    }
}