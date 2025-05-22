#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hashilus.Setting
{
    public class SettingInt : SettingField<int>
    {
        public SettingInt(string description, int defaultValue) : base(description, defaultValue) { }

        public static implicit operator int(SettingInt v)
        {
            return v.Value;
        }

        protected override int Parse(string valueString)
        {
            return int.Parse(valueString);
        }

        public override void OnGUI()
        {
#if UNITY_EDITOR
            Value = EditorGUILayout.IntField(Description, Value);
            if (!Value.Equals(previousValue))
            {
                Settings.SaveAsUserLocal();
                previousValue = Value;
            }
#endif
        }
    }
}