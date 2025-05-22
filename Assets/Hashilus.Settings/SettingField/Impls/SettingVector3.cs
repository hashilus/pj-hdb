#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hashilus.Setting
{
    public class SettingVector3 : SettingField<Vector3>
    {
        static readonly Regex parenthesisRegex = new Regex(@"[\(\)]+");
        static readonly Regex delimiterRegex = new Regex(@"[, ]+");

        public SettingVector3(string description, Vector3 defaultValue) : base(description, defaultValue) { }

        protected override Vector3 Parse(string valueString)
        {
            var sanitizedString = parenthesisRegex.Replace(valueString.Trim(), "");
            var values = delimiterRegex.Split(sanitizedString)
                                       .Select(s => float.Parse(s));
            return new Vector3(values.ElementAt(0), values.ElementAt(1), values.ElementAt(2));
        }

        public override void OnGUI()
        {
#if UNITY_EDITOR
            Value = EditorGUILayout.Vector3Field(Description, Value);
            if (!Value.Equals(previousValue))
            {
                Settings.SaveAsUserLocal();
                previousValue = Value;
            }
#endif
        }

        public override string ToString()
        {
            return Value.x + ", " + Value.y + ", " + Value.z;
        }
    }
}