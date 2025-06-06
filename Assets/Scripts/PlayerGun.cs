using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField]
    Bullet bulletPrefab;

    [SerializeField]
    Transform shootingOrigin;

    [SerializeField]
    float shootingForce = 500.0f;

    [Header("Effects")]
    [SerializeField]
    ParticleSystem smokeParticle;

    [SerializeField]
    ParticleSystem gasParticle;

    [SerializeField]
    ParticleSystem waterParticle;

    [Header("Tracker")]
    [SerializeField]
    Transform calibratedTracker;

    [SerializeField]
    AirController airController;

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
        else
        {
            UpdateByMouse();
        }

        UpdatePose();
        UpdateShooting();
    }

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

        targetLocalRotation = cameraLocalTrackerRotation;
    }

    void UpdateButtonByTracker()
    {
        var isHardwarePlayer1 = Player.ID == PlayerID.Player2; // ハードウェアの Player1 はゲーム内と左右逆
        var playerSection = isHardwarePlayer1 ? AirBlowPermission.PlayerSelection.Player1 : AirBlowPermission.PlayerSelection.Player2;
        var buttonName = isHardwarePlayer1 ? "Fire1" : "Fire2";

        if (AirBlowPermission.CanBlow(playerSection))
        {
            if (Input.GetButtonDown(buttonName)) { airController.StartBlow(); }
            else if (Input.GetButtonUp(buttonName)) { airController.StopBlow(); }

            isShooting = Input.GetButton(buttonName);
        }
        else
        {
            isShooting = false;
        }
    }

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

        var viewportMousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        var normalizedMousePosition = new Vector2(
            viewportMousePosition.x * 2.0f - 1.0f,
            viewportMousePosition.y * 2.0f - 1.0f);

        var deltaMousePosition = normalizedMousePosition - prevNormalizedMousePosition;
        prevNormalizedMousePosition = normalizedMousePosition;

        if (Input.GetMouseButton(1))
        {
            var euler = targetLocalRotation.eulerAngles;
            euler.x = NormalizeAngle(euler.x);
            euler.y = NormalizeAngle(euler.y);
            euler.z = NormalizeAngle(euler.z);

            euler.y += deltaMousePosition.x * Settings.Gun.MouseRotationSensitivity;
            euler.x -= deltaMousePosition.y * Settings.Gun.MouseRotationSensitivity;

            euler.x = Mathf.Clamp(euler.x, Settings.Gun.HorizontalLimitAngle.Value.x, Settings.Gun.HorizontalLimitAngle.Value.y);
            euler.y = Mathf.Clamp(euler.y, Settings.Gun.VerticalLimitAngle.Value.x, Settings.Gun.VerticalLimitAngle.Value.y);

            targetLocalRotation = Quaternion.Euler(euler);
        }
        else
        {
            var x = normalizedMousePosition.x * Settings.Gun.MovingRange.Value.x;
            var y = Mathf.Clamp(
                targetLocalPosition.y + deltaMousePosition.y * Settings.Gun.MouseVirticalMovingSensitivity,
                -Settings.Gun.MovingRange.Value.y,
                Settings.Gun.MovingRange.Value.y);
            targetLocalPosition = new Vector3(x, y, 0f);

            targetLocalRotation = Quaternion.identity;
        }
    }

    void UpdateButtonByMouse(bool isInputForThisPlayer)
    {
        if (isInputForThisPlayer)
        {
            isShooting = Input.GetMouseButton(0);
        }
        else
        {
            isShooting = false;
        }
    }

    void UpdatePose()
    {
        transform.SetLocalPositionAndRotation(
            Vector3.Lerp(transform.localPosition, targetLocalPosition, Settings.Gun.MovingInterpolation),
            Quaternion.Slerp(transform.localRotation, targetLocalRotation, Settings.Gun.RotationInterpolation));

        CorrectAngle();

        targetLocalPosition.y = Mathf.Lerp(targetLocalPosition.y, 0.0f, Settings.Gun.VirticalRestoringInterpolation);
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
    }

    void StopShootingEffect()
    {
        var emission = smokeParticle.emission;
        emission.rateOverTime = 0f;

        emission = gasParticle.emission;
        emission.rateOverTime = 0f;

        emission = waterParticle.emission;
        emission.rateOverTime = 0f;
    }
}