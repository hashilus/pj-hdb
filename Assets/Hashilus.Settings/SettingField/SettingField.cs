using UnityEngine;

namespace Hashilus.Setting
{
    public abstract class SettingField<ValueT> : ISettingField
    {
        public string Key { get; set; }
        public virtual ValueT Value { get; set; }
        public string Description { get; private set; }
        public ValueT DefaultValue { get; private set; }

#if UNITY_EDITOR
        protected ValueT previousValue;
#endif

        public SettingField(string description, ValueT defaultValue)
        {
            Description = description;
            Value = DefaultValue = defaultValue;
        }

        public void SetByString(string valueString)
        {
            try
            {
                Value = Parse(valueString);
            }
            catch
            {
                Debug.LogWarning(Key + "の値" + valueString + "が" + typeof(ValueT).Name + "として読み込めなかったため" + DefaultValue + "を使用します。");
                Value = DefaultValue;
            }

#if UNITY_EDITOR
            previousValue = Value;
#endif
        }

        protected abstract ValueT Parse(string valueString);

        public static implicit operator ValueT(SettingField<ValueT> v)
        {
            return v.Value;
        }

        public bool EqualsDefault()
        {
            return Value.Equals(DefaultValue);
        }

        public abstract void OnGUI();

        public override string ToString()
        {
            return Value.ToString();
        }

        public void Reset()
        {
            Value = DefaultValue;
        }
    }
}