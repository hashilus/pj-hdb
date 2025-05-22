namespace Hashilus.Setting
{
    public interface ISettingField
    {
        string Key { get; set; }
        string Description { get; }
        void SetByString(string valueString);
        string ToString();
        void OnGUI();
        bool EqualsDefault();
        void Reset();
    }
}