using UnityEngine;
using ChocDino.PartyIO;

public class PlayerGun : MonoBehaviour
{
    // ---------- Shooting ----------
    [Header("Shooting")]
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] Transform shootingOrigin;
    [SerializeField] float shootingForce = 500.0f;

    // ---------- Effects ----------
    [Header("Effects")]
    [SerializeField] ParticleSystem smokeParticle;
    [SerializeField] ParticleSystem gasParticle;
    [SerializeField] ParticleSystem waterParticle;
    [SerializeField] Animator waterSoundAnimator;
    [SerializeField] AudioSource waterHitSoundAudioSource;
    [SerializeField] Animator waterHitSoundAnimator;

    // ---------- Tracker ----------
    [Header("Tracker")]
    [SerializeField] Transform calibratedTracker;
    [SerializeField] AirController airController;

    // ---------- Mouse Party ----------
    [Header("Mouse Party")]
    [SerializeField] bool useMouseParty = true;
    [SerializeField, Min(0)] int mousePartyDeviceIndex = 0; // 1P=0, 2P=1

    // ---------- State ----------
    public Player Player { get; private set; }

    bool isShooting;
    bool isPrevShooting;
    float shootTime;

    Vector3 targetLocalPosition;
    Quaternion targetLocalRotation;

    Vector2 prevNormalizedMousePosition;
    Vector3 prevCameraLocalTrackerPosition;

    static float NormalizeAngle(float angle) => Mathf.Repeat(angle + 180f, 360f) - 180f;

    void Awake()
    {
        Player = GetComponentInParent<Player>();
    }

    void OnDisable()
    {
        isShooting = false;
        if (Settings.System.IsUseTracker) airController.StopBlow();
    }

    void Start()
    {
        transform.GetLocalPositionAndRotation(out targetLocalPosition, out targetLocalRotation);
        StopShootingEffect();
    }

    void Update()
    {
        if (Settings.System.IsUseTracker)
        {
            UpdateByTracker();
        }
        else if (useMouseParty)
        {
            UpdateByMousePartyBridge();
        }
        else
        {
            UpdateByMouse(); // 旧：単一マウス
        }

        UpdatePose();
        UpdateShooting();
    }

    // ===== Mouse Party via Bridge =====
    void UpdateByMousePartyBridge()
    {
        // 1台しかない時は 1P の index=0 のみ成功し、2P は失敗→return で自然に無効化
        if (!MousePartyInputBridge.TryGetViewport(mousePartyDeviceIndex, out var vp01))
            return;

        float nx  = vp01.x * 2f - 1f;  // -1..+1
        float y01 = vp01.y;            // 0..1

        // 位置
        float x = nx * Settings.Gun.MovingRange.Value.x;
        float y = y01;
        targetLocalPosition = new Vector3(x, y, 0f);

        // 俯仰角（Y=0→下、0.5→水平、Y=1→上）
        float minPitch = Settings.Gun.VerticalLimitAngle.Value.x;
        float maxPitch = Settings.Gun.VerticalLimitAngle.Value.y;
        float pitchDeg = Mathf.Lerp(minPitch, maxPitch, y01);
        targetLocalRotation = Quaternion.Euler(pitchDeg, 0f, 0f);

        // 発射
        isShooting = MousePartyInputBridge.GetButton(mousePartyDeviceIndex, MouseButton.Left);
    }

    // ===== Old single mouse fallback =====
    void UpdateByMouse()
    {
        var altPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        var inputPlayer = altPressed ? PlayerID.Player2 : PlayerID.Player1;
        var isInputForThisPlayer = Player.ID == inputPlayer;

        UpdateTargetPoseByMouse(isInputForThisPlayer);
        UpdateButtonByMouse(isInputForThisPlayer);
    }

    void UpdateTargetPoseByMouse(bool isInputForThisPlayer)
    {
        if (!isInputForThisPlayer) return;

        var vp = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        float nx  = vp.x * 2f - 1f;
        float y01 = Mathf.Clamp01(vp.y);

        float x = nx * Settings.Gun.MovingRange.Value.x;
        float y = y01;
        targetLocalPosition = new Vector3(x, y, 0f);

        float minPitch = Settings.Gun.VerticalLimitAngle.Value.x;
        float maxPitch = Settings.Gun.VerticalLimitAngle.Value.y;
        float pitchDeg = Mathf.Lerp(minPitch, maxPitch, y01);
        targetLocalRotation = Quaternion.Euler(pitchDeg, 0f, 0f);

        prevNormalizedMousePosition = new Vector2(nx, y01 * 2f - 1f);
    }

    void UpdateButtonByMouse(bool isInputForThisPlayer)
    {
        isShooting = isInputForThisPlayer && Input.GetMouseButton(0);
    }

    // ===== Tracker (既存) =====
    void UpdateByTracker()
    {
        UpdateTargetPoseByTracker();
        UpdateButtonByTracker();
    }

    void UpdateTargetPoseByTracker()
    {
        var cameraLocalTrackerPosition = Camera.main.transform.InverseTransformPoint(calibratedTracker.position);
        var cameraLocalTrackerRotation = Quaternion.Inverse(Camera.main.transform.rotation) * calibratedTracker.rotation;

        var deltaTrackerPosition = cameraLocalTrackerPosition - prevCameraLocalTrackerPosition;
        prevCameraLocalTrackerPosition = cameraLocalTrackerPosition;

        var x = cameraLocalTrackerPosition.x * Settings.Gun.MovingRange.Value.x;
        var y = Mathf.Clamp(
            targetLocalPosition.y + deltaTrackerPosition.y * Settings.Gun.TrackerVirticalMovingSensitivity,
            -Settings.Gun.MovingRange.Value.y,
            Settings.Gun.MovingRange.Value.y);
        targetLocalPosition = new Vector3(x, y, 0f);

        var euler = cameraLocalTrackerRotation.eulerAngles;
        euler.x = NormalizeAngle(euler.x);
        euler.y = NormalizeAngle(euler.y);
        euler.z = NormalizeAngle(euler.z);

        euler.x = Mathf.Clamp(euler.x, Settings.Gun.HorizontalLimitAngle.Value.x, Settings.Gun.HorizontalLimitAngle.Value.y);
        euler.y = Mathf.Clamp(euler.y, Settings.Gun.VerticalLimitAngle.Value.x, Settings.Gun.VerticalLimitAngle.Value.y);

        targetLocalRotation = Quaternion.Euler(euler);
    }

    void UpdateButtonByTracker()
    {
        var isHardwarePlayer1 = Player.ID == PlayerID.Player2; // ハードウェアの Player1 はゲーム内と左右逆
        var playerSection = isHardwarePlayer1 ? AirBlowPermission.PlayerSelection.Player1 : AirBlowPermission.PlayerSelection.Player2;
        var buttonName = isHardwarePlayer1 ? "Fire1" : "Fire2";

        if (!AirBlowPermission.CanBlow(playerSection))
        {
            isShooting = false;
            return;
        }

        var isButtonPressed = Input.GetButton(buttonName);
        if (isButtonPressed && !isShooting)
        {
            isShooting = true;
            airController.StartBlow();
        }
        else if (!isButtonPressed && isShooting)
        {
            isShooting = false;
            airController.StopBlow();
        }
    }

    // ===== Pose / Shooting =====
    void UpdatePose()
    {
        transform.SetLocalPositionAndRotation(
            Vector3.Lerp(transform.localPosition, targetLocalPosition, Settings.Gun.MovingInterpolation),
            Quaternion.Slerp(transform.localRotation, targetLocalRotation, Settings.Gun.RotationInterpolation));

        CorrectAngle();

        if (Settings.System.IsUseTracker)
        {
            targetLocalPosition.y = Mathf.Lerp(targetLocalPosition.y, 0.0f, Settings.Gun.VirticalRestoringInterpolation);
        }
    }

    void CorrectAngle()
    {
        var center = Camera.main.transform;
        var placement = center.transform.InverseTransformPoint(transform.position).x;
        var correctionAngle = Quaternion.Euler(0f, placement * Settings.Gun.AngleCorrection, 0f);
        transform.localRotation *= correctionAngle;
    }

    void UpdateShooting()
    {
        if (isShooting)
        {
            if (!isPrevShooting) { StartShootingEffect(); }

            if (Time.time >= shootTime + Settings.Gun.ShootingInterval)
            {
                shootTime = Time.time;
                ShootBullet();
            }
        }
        else
        {
            if (isPrevShooting) { StopShootingEffect(); }
        }

        isPrevShooting = isShooting;
    }

    void ShootBullet()
    {
        var bullet = Instantiate(bulletPrefab, shootingOrigin.position, shootingOrigin.rotation);
        bullet.Shooter = Player.ID;
        bullet.OnHit += OnHitBullet;

        bullet.transform.localScale *= Settings.Bullet.RadiusFactor;
        bullet.GetComponent<Renderer>().enabled = Settings.Bullet.ShowCollider;

        var rigidbody = bullet.GetComponent<Rigidbody>();
        rigidbody.AddForce(transform.forward * shootingForce * Settings.Gun.ShootingForceFactor);
    }

    void StartShootingEffect()
    {
        var emission = smokeParticle.emission;
        emission.rateOverTime = 8.0f;

        emission = gasParticle.emission;
        emission.rateOverTime = 10.0f;

        emission = waterParticle.emission;
        emission.rateOverTime = 60.0f;

        waterSoundAnimator.SetBool("IsShooting", true);
    }

    void StopShootingEffect()
    {
        var emission = smokeParticle.emission;
        emission.rateOverTime = 0f;

        emission = gasParticle.emission;
        emission.rateOverTime = 0f;

        emission = waterParticle.emission;
        emission.rateOverTime = 0f;

        waterSoundAnimator.SetBool("IsShooting", false);
    }

    void OnHitBullet(Bullet bullet, GameObject other)
    {
        waterHitSoundAudioSource.transform.position = bullet.transform.position;
        waterHitSoundAnimator.SetTrigger("Hit");
    }
}
