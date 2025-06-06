#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hashilus.Setting
{
    public class SettingVector2 : SettingField<Vector2>
    {
        static readonly Regex parenthesisRegex = new Regex(@"[\(\)]+");
        static readonly Regex delimiterRegex = new Regex(@"[, ]+");

        public SettingVector2(string description, Vector2 defaultValue) : base(description, defaultValue) { }

        protected override Vector2 Parse(string valueString)
        {
            var sanitizedString = parenthesisRegex.Replace(valueString.Trim(), "");
            var values = delimiterRegex.Split(sanitizedString)
                                       .Select(float.Parse);
            return new Vector2(values.ElementAt(0), values.ElementAt(1));
        }

        public override void OnGUI()
        {
#if UNITY_EDITOR
            SaveValueOnGUI(EditorGUILayout.Vector2Field(Description, Value));
#endif
        }

        public override string ToString()
        {
            return $"{Value.x.ToString()}, {Value.y.ToString()}";
        }
    }
}
