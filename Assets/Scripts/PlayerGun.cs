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
    AirController airController;

    public Player Player { get; private set; }

    bool isShooting;
    bool isPrevShooting;
    float shootTime;

    void Awake()
    {
        Player = GetComponentInParent<Player>();
    }

    void Start()
    {
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

        UpdateShooting();
    }

    void UpdateByTracker()
    {
        UpdatePoseByTracker();
        UpdateButtonByTracker();
    }

    void UpdatePoseByTracker()
    {
        //CorrectAngle();
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

        UpdatePoseByMouse(isInputForThisPlayer);
        UpdateButtonByMouse(isInputForThisPlayer);
    }

    void UpdatePoseByMouse(bool isInputForThisPlayer)
    {
        if (!isInputForThisPlayer) return;

        transform.localRotation = Quaternion.identity;

        CorrectAngle();
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