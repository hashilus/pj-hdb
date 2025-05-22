using Hashilus.Setting;
using UnityEngine;

public partial class Settings   
{
    public class System
    {
        // public static SettingFloat NextPlayWaitTime = new("体験終了後、ファンファーレを流すまでの待機時間（sec）※5秒単位推奨", 10f);
        // public static SettingFloat MouseCursorHiddenTime = new("マウスカーソルを動かさない時に非表示とする時間（sec）", 1.5f);
    }

    public class Calibration
    {
        // public static SettingVector3 SideBoxPosition = new ("サイドボックスのHRUP座面からの相対距離", new Vector3(-0.66f, 0.03f, 0.6f));
        // public static SettingFloat MotionDetectionPositionThreshold = new ("HMD検知の閾値(位置)", 0.01f);
        // public static SettingFloat MotionDetectionRotationThreshold = new ("HMD検知の閾値(回転)", 0.2f);
        // public static SettingFloat MontionDetectionInactiveTime = new ("HMD検知の非アクティブ時間", 3f);
        // public static SettingFloat AllowableDiffDistance = new ("許容誤差(位置)", 0.03f);
        // public static SettingFloat AllowableDiffAngle = new ("許容誤差(角度)", 2f);
        
     
        public static SettingFloat TrackerTransferCoefficient = new ("トラッカーの移動量をどれだけ空間内に反映かの係数", 1f);
        public static SettingVector3 ControllerOffset = new("コントローラーのオフセット", new Vector3(0,-0.5f,-1f));
    }
    
    public class InGame
    {
    }
}
