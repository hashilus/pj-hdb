#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace Hashilus.Setting
{
    public class SettingDirectory : SettingField<string>
    {
        DirectoryInfo info;
        public DirectoryInfo Info
        {
            get
            {
                info.Refresh();
                return info;
            }
        }

        public override string Value
        {
            get { return base.Value; }
            set
            {
                base.Value = value;
                info = new DirectoryInfo(Value);
            }
        }

        public SettingDirectory(string description, string defaultValue) : base(description, defaultValue) { }

        public void CreateIfDoesntExists()
        {
            info.Refresh();
            if (!info.Exists)
            {
                info.Create();
            }
        }

        public static implicit operator string(SettingDirectory v)
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
            Value = EditorGUILayout.TextField(Description, Value);
            if (!Value.Equals(previousValue))
            {
                Settings.SaveAsUserLocal();
                previousValue = Value;
            }
#endif
        }
    }
}