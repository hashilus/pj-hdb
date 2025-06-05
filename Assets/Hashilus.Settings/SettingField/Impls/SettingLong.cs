#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hashilus.Setting
{
    public class SettingLong : SettingField<long>
    {
        public SettingLong(string description, long defaultValue) : base(description, defaultValue) { }

        public static implicit operator long(SettingLong v)
        {
            return v.Value;
        }

        protected override long Parse(string valueString)
        {
            return long.Parse(valueString);
        }

        public override void OnGUI()
        {
#if UNITY_EDITOR
            SaveValueOnGUI(EditorGUILayout.LongField(Description, Value));
#endif
        }
    }
}