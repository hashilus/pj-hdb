using UnityEngine;
using ChocDino.PartyIO;

public class PlayerGun : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] Transform shootingOrigin;
    [SerializeField] float shootingForce = 500.0f;

    [Header("Effects")]
    [SerializeField] ParticleSystem smokeParticle;
    [SerializeField] ParticleSystem gasParticle;
    [SerializeField] ParticleSystem waterParticle;
    [SerializeField] Animator waterSoundAnimator;
    [SerializeField] AudioSource waterHitSoundAudioSource;
    [SerializeField] Animator waterHitSoundAnimator;

    [Header("Tracker")]
    [SerializeField] Transform calibratedTracker;
    [SerializeField] AirController airController;

    [Header("MouseParty")]
    [SerializeField] MousePartyInputRouter mouseRouter;
    [SerializeField] bool useMouseParty = true;

    public Player Player { get; private set; }

    bool isShooting;
    bool isPrevShooting;
    float shootTime;

    Vector3 targetLocalPosition;
    Quaternion targetLocalRotation;

    Vector2 prevNormalizedMousePosition;
    Vector3 prevCameraLocalTrackerPosition;

    // MouseParty用
    Vector3 partyMouseScreenPos = Vector3.zero;
    bool partyMouseInitialized;

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
        else
        {
            if (useMouseParty && mouseRouter != null && mouseRouter.ConnectedCount > 0)
                UpdateByMouseParty();
            else
                UpdateByMouse();
        }

        UpdatePose();
        UpdateShooting();
    }

    //==================== Tracker ====================
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
        var isHardwarePlayer1 = Player.ID == PlayerID.Player2;
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

    //==================== MouseParty ====================
    void UpdateByMouseParty()
    {
        int idx = (Player.ID == PlayerID.Player1) ? 0 : 1;
        var mouse = mouseRouter.GetMouseForIndex(idx);
        if (mouse == null)
        {
            isShooting = false;
            return;
        }

        if (!partyMouseInitialized)
        {
            partyMouseScreenPos = new Vector3(Camera.main.pixelWidth * 0.5f, Camera.main.pixelHeight * 0.5f, 0f);
            partyMouseInitialized = true;
        }

        // 座標更新
        Vector3 screenMin = Vector3.zero;
        Vector3 screenMax = new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0f);
        Vector3 newPos = partyMouseScreenPos;

        if (mouse.IsPositionAbsolute())
        {
            newPos = mouse.PositionDelta;
        }
        else if (mouse.PositionDelta != Vector3.zero)
        {
            newPos += new Vector3(mouse.PositionDelta.x, -mouse.PositionDelta.y, 0f);
        }

        if (newPos != partyMouseScreenPos)
        {
            newPos = Vector3.Max(newPos, screenMin);
            newPos = Vector3.Min(newPos, screenMax);
            partyMouseScreenPos = newPos;
        }

        // 座標変換
        var viewport = new Vector2(
            partyMouseScreenPos.x / Camera.main.pixelWidth,
            partyMouseScreenPos.y / Camera.main.pixelHeight
        );
        var nmp = new Vector2(viewport.x * 2f - 1f, viewport.y * 2f - 1f);

        var x = nmp.x * Settings.Gun.MovingRange.Value.x;
        var y = Mathf.Clamp01(0.5f + 0.5f * nmp.y);
        targetLocalPosition = new Vector3(x, y, 0f);

        const float pitchAtY0 = 25f;
        const float pitchAtY1 = -35f;
        float pitch = Mathf.Lerp(pitchAtY0, pitchAtY1, y);
        targetLocalRotation = Quaternion.Euler(pitch, 0f, 0f);

        // ボタン状態（押下維持で連射）
        try
        {
            isShooting = mouse.IsPressed(MouseButton.Left);
        }
        catch
        {
            if (mouse.WasPressedThisFrame(MouseButton.Left)) isShooting = true;
            if (mouse.WasReleasedThisFrame(MouseButton.Left)) isShooting = false;
        }
    }

    //==================== 単一マウス（フォールバック） ====================
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
        var nmp = new Vector2(viewportMousePosition.x * 2f - 1f, viewportMousePosition.y * 2f - 1f);

        var x = nmp.x * Settings.Gun.MovingRange.Value.x;
        var y = Mathf.Clamp01(0.5f + 0.5f * nmp.y);
        targetLocalPosition = new Vector3(x, y, 0f);

        const float pitchAtY0 = 25f;
        const float pitchAtY1 = -35f;
        float pitch = Mathf.Lerp(pitchAtY0, pitchAtY1, y);
        targetLocalRotation = Quaternion.Euler(pitch, 0f, 0f);

        prevNormalizedMousePosition = nmp;
    }

    void UpdateButtonByMouse(bool isInputForThisPlayer)
    {
        isShooting = isInputForThisPlayer && Input.GetMouseButton(0);
    }

    //==================== 共通処理 ====================
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
            if (!isPrevShooting) StartShootingEffect();

            if (Time.time >= shootTime + Settings.Gun.ShootingInterval)
            {
                shootTime = Time.time;
                ShootBullet();
            }
        }
        else
        {
            if (isPrevShooting) StopShootingEffect();
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
