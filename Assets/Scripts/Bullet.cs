using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    GameObject hitParticlePrefab;

    [SerializeField]
    float hitParticleLifetime = 1.0f;

    [SerializeField, Range(0f, 1f)]
    float hitParticleSpawnChance = 0.5f;

    [SerializeField]
    AudioClip[] deskHitSounds;

    [SerializeField]
    AudioClip[] bowlHitSounds;

    public PlayerID Shooter { get; set; }

    SphereCollider sphereCollider;
    AudioSource audioSource;
    bool hasCollided = false;

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        audioSource = gameObject.GetComponent<AudioSource>();

        Destroy(gameObject, Settings.Bullet.Lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return; // すでに衝突していたら何もしない
        hasCollided = true; // 初回衝突としてマーク

        SpawnParticle();

        StartCoroutine(ExpandColliderTemporarily());

        switch (collision.gameObject.tag)
        {
            case "DESK":
                PlayDeskHitSound(collision.gameObject);
                break;
            case "BOWL":
                PlayBowlHitSound(collision.gameObject);
                break;
            case "Fire":
                PlayFireExtinguishSound(collision.gameObject);
                break;
        }

        Destroy(gameObject, Settings.Bullet.LifetimeOnHit);
    }

    void SpawnParticle()
    {
        if (!hitParticlePrefab) return;

        if (Random.value <= hitParticleSpawnChance)
        {
            var particle = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
            Destroy(particle, hitParticleLifetime);
        }
    }

    System.Collections.IEnumerator ExpandColliderTemporarily()
    {
        if (!sphereCollider) yield break;

        var originalRadius = sphereCollider.radius;
        sphereCollider.radius = originalRadius * Settings.Bullet.ImpactRadiusFactor;

        yield return new WaitForSeconds(Settings.Bullet.ImpactDuration);

        if (sphereCollider) sphereCollider.radius = originalRadius;
    }

    void PlayDeskHitSound(GameObject other)
    {
        int idx = Random.Range(0, deskHitSounds.Length);
        audioSource.clip = deskHitSounds[idx];
        audioSource.volume = 0.15f;
        audioSource.Play();
    }

    void PlayBowlHitSound(GameObject other)
    {
        int idx = Random.Range(0, bowlHitSounds.Length);
        audioSource.clip = bowlHitSounds[idx];
        audioSource.volume = 0.30f;
        audioSource.Play();
    }

    void PlayFireExtinguishSound(GameObject other)
    {
        var fire = other.GetComponent<FireController>();
        if (!fire) return;

        if (fire.fireExSE.isPlaying) return;
        fire.fireExSE.Play();
    }
}
