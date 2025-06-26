using Hashilus.Setting;
using UnityEngine;

public partial class Settings
{
    public class System
    {
        public static SettingBool IsUseTracker = new("トラッカーを使用するかどうか", false);
        public static SettingString HACCAddressL = new("HACC(L)のIPアドレス", "192.168.5.101");
        public static SettingInt HACCPortL = new("HACC(L)のポート", 5555);
        public static SettingString HACCAddressR = new("HACC(R)のIPアドレス", "192.168.5.102");
        public static SettingInt HACCPortR = new("HACC(R)のポート", 5555);
        public static SettingFloat AirBlowToWeakenWait = new("空気吹きを弱めるまでの待機時間(秒)", 2f);
        public static SettingFloat TrackerLostCheckDuration = new("トラッカーが見失われたとみなすまでの時間(秒)", 1f);
        public static SettingBool UsedSpaceCalibrator = new("スペースキャリブレーターを使ってトラッカーのキャリブレーションを実施した【旧トラッカーでは必須】", false);
        public static SettingFloat ControllerLimitAngle = new("コントローラーの角度制限", 80f);
    }

    public class Calibration
    {
        public static SettingFloat TrackerTransferCoefficient = new("トラッカーの移動量をどれだけ空間内に反映するかの係数", 0.9f);
        public static SettingVector3 ControllerOffsetL = new("Lコントローラーのオフセット", new Vector3(-0.2f, -0.8f, -0.7f));
        public static SettingVector3 ControllerOffsetR = new("Rコントローラーのオフセット", new Vector3(0.2f, -0.8f, -0.7f));
        public static SettingFloat ControllerXRotationOffset = new("コントローラーのX軸回転オフセット", 0f);
    }

    public class Bullet
    {
        public static SettingFloat Lifetime = new("弾の寿命 (秒)", 2.0f);
        public static SettingFloat LifetimeOnHit = new("ヒット後の弾の寿命 (秒)", 1.0f);
        public static SettingFloat RadiusFactor = new("弾のサイズ係数", 1.0f);
        public static SettingFloat ImpactRadiusFactor = new("ヒット後の弾のサイズ係数", 1.2f);
        public static SettingFloat ImpactDuration = new("ヒット後の弾のサイズ変更継続時間 (秒)", 0.2f);
        public static SettingBool ShowCollider = new("弾のコライダを表示するか", false);
    }

    public class Gun
    {
        public static SettingFloat ShootingForceFactor = new("発射時の力の係数", 1.0f);
        public static SettingFloat ShootingInterval = new("連射間隔 (秒)", 0.05f);
        public static SettingVector2 MovingRange = new("銃の移動範囲 (m)", new Vector2(1.0f, 0.1f));
        public static SettingVector2 HorizontalLimitAngle = new("銃のピッチ回転制限角度 (度)", new Vector2(-60.0f, 60.0f));
        public static SettingVector2 VerticalLimitAngle = new("銃のヨー回転制限角度 (度)", new Vector2(-100.0f, 100.0f));
        public static SettingFloat VirticalRestoringInterpolation = new("銃の上下位置減衰係数 (0～1)", 0.02f);
        public static SettingFloat MovingInterpolation = new("銃の移動補間係数 (0～1)", 0.9f);
        public static SettingFloat RotationInterpolation = new("銃の回転補間係数 (0～1)", 0.95f);
        public static SettingFloat AngleCorrection = new("左右移動に対する角度補正 (度/m)", 40.0f);
        
        public static SettingFloat MouseRotationSensitivity = new("マウスによる銃の回転感度", 90.0f);
        public static SettingFloat MouseVirticalMovingSensitivity = new("マウスによる銃の上下移動感度", 0.3f);

        public static SettingFloat TrackerVirticalMovingSensitivity = new("トラッカーによる銃の上下移動感度", 0.3f);
    }
}
