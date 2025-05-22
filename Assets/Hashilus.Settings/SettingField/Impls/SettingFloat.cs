#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hashilus.Setting
{
    public class SettingFloat : SettingField<float>
    {
        public SettingFloat(string description, float defaultValue) : base(description, defaultValue) { }

        protected override float Parse(string valueString)
        {
            return float.Parse(valueString);
        }

        public override void OnGUI()
        {
#if UNITY_EDITOR
            Value = EditorGUILayout.FloatField(Description, Value);
            if (!Value.Equals(previousValue))
            {
                Settings.SaveAsUserLocal();
                previousValue = Value;
            }
#endif
        }
    }
}