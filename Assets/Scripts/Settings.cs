using Hashilus.Setting;
using UnityEngine;

public partial class Settings   
{
    public class System
    {
        public static SettingBool IsUseTracker = new("トラッカーを使用するかどうか", false);
        public static SettingString HACCAddressL = new("HACC(L)のIPアドレス", "127.0.0.1");
        public static SettingInt HACCPortL = new("HACC(L)のポート", 5555);
        public static SettingString HACCAddressR = new("HACC(R)のIPアドレス", "127.0.0.1");
        public static SettingInt HACCPortR = new("HACC(R)のポート", 5555);
        public static SettingFloat AirBlowToWeakenWait = new("空気吹きを弱めるまでの待機時間(秒)", 2f);
        public static SettingFloat TrackerLostCheckDuration = new("トラッカーが見失われたとみなすまでの時間(秒)", 1f);
        public static SettingFloat TargetScreenSize = new("レティクルの画面上での目標サイズ（画面の割合）", 0.23f);
        public static SettingFloat ReticleMoveTime = new("レティクルの移動時間", 0.1f);
    }

    public class Calibration
    {
        public static SettingFloat TrackerTransferCoefficient = new ("トラッカーの移動量をどれだけ空間内に反映するかの係数", 1f);
        public static SettingVector3 ControllerOffsetL = new("Lコントローラーのオフセット", new Vector3(-1f,-0.5f,-1f));
        public static SettingVector3 ControllerOffsetR = new("Rコントローラーのオフセット", new Vector3(1f,-0.5f,-1f));
        public static SettingFloat ControllerXRotationOffset = new("コントローラーのX軸回転オフセット", -5f);
    }

    public class ControllerAngle
    {
        public static SettingFloat XAxisMinAngle = new("コントローラーのX軸最小角度", -45f);
        public static SettingFloat XAxisMaxAngle = new("コントローラーのX軸最大角度", 45f);
        public static SettingFloat YAxisMinAngle = new("コントローラーのY軸最小角度", -45f);
        public static SettingFloat YAxisMaxAngle = new("コントローラーのY軸最大角度", 45f);
    }
}
