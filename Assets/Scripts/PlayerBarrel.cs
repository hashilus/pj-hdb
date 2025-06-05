using UnityEngine;

public class PlayerBarrel : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    float shootingForce = 500.0f;

    [SerializeField]
    float shootingInterval = 0.05f;

    [Header("Effects")]
    [SerializeField]
    GameObject hitParticlePrefab;

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

    void Update()
    {
        if (Settings.System.IsUseTracker)
        {
            UpdatePoseByTracker();
            UpdateButtonByTracker();
        }
        else
        {
            UpdatePoseByMouse();
            UpdateButtonByMouse();
        }

        UpdateShooting();
    }

    void UpdatePoseByTracker()
    {

    }

    void UpdateButtonByTracker()
    {
        var playerSection = Player.Type == PlayerType.Player1
            ? AirBlowPermission.PlayerSelection.Player1
            : AirBlowPermission.PlayerSelection.Player2;
        var buttonName = Player.Type == PlayerType.Player1 ? "Fire1" : "Fire2";

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

    void UpdatePoseByMouse()
    {
    }

    void UpdateButtonByMouse()
    {

    }

    void UpdateShooting()
    {
        if (isShooting)
        {
            if (!isPrevShooting) { StartShootingEffect(); }

            if (Time.time >= shootTime + shootingInterval)
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