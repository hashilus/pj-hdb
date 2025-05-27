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
    }

    public class Calibration
    {
        // public static SettingVector3 SideBoxPosition = new ("サイドボックスのHRUP座面からの相対距離", new Vector3(-0.66f, 0.03f, 0.6f));
        // public static SettingFloat MotionDetectionPositionThreshold = new ("HMD検知の閾値(位置)", 0.01f);
        // public static SettingFloat MotionDetectionRotationThreshold = new ("HMD検知の閾値(回転)", 0.2f);
        // public static SettingFloat MontionDetectionInactiveTime = new ("HMD検知の非アクティブ時間", 3f);
        // public static SettingFloat AllowableDiffDistance = new ("許容誤差(位置)", 0.03f);
        // public static SettingFloat AllowableDiffAngle = new ("許容誤差(角度)", 2f);
        
     
        public static SettingFloat TrackerTransferCoefficient = new ("トラッカーの移動量をどれだけ空間内に反映するかの係数", 1f);
        public static SettingVector3 ControllerOffsetL = new("Lコントローラーのオフセット", new Vector3(-1f,-0.5f,-1f));
        public static SettingVector3 ControllerOffsetR = new("Rコントローラーのオフセット", new Vector3(1f,-0.5f,-1f));
        public static SettingFloat ControllerXRotationOffset = new("コントローラーのX軸回転オフセット", -5f);
    }
}
