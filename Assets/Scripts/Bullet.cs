using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    GameObject hitParticlePrefab;

    [SerializeField]
    float hitParticleLifetime = 1.0f;

    [SerializeField, Range(0f, 1f)]
    float hitParticleSpawnChance = 0.5f;

    public PlayerID Shooter { get; set; }

    SphereCollider sphereCollider;
    bool hasCollided = false; // ← これが重要

    public AudioClip[] deskhitSound; // ヒット時のサウンド
    public AudioClip[] bowlhitSound; // ヒット時のサウンド
    
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();

        Destroy(gameObject, Settings.Bullet.Lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return; // すでに衝突していたら何もしない
        hasCollided = true; // 初回衝突としてマーク

        // パーティクル生成（確率）
        if (hitParticlePrefab && Random.value <= hitParticleSpawnChance)
        {
            var particle = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
            Destroy(particle, hitParticleLifetime);
        }

        // 拡大コライダー処理
        StartCoroutine(ExpandColliderTemporarily());

        AudioSource deskAudio = gameObject.GetComponent<AudioSource>();
        if (collision.gameObject.CompareTag("DESK"))
        {
            int idx = Random.Range(0, deskhitSound.Length);
            deskAudio.clip = deskhitSound[idx];
            deskAudio.volume = 0.15f;
            deskAudio.Play();
        }

        if (collision.gameObject.CompareTag("BOWL"))
        {
            int idx = Random.Range(0, bowlhitSound.Length);
            deskAudio.clip = bowlhitSound[idx];
            deskAudio.volume = 0.30f;
            deskAudio.Play();
        }

        //火にヒットした時に消火SEを出す
        if (collision.gameObject.CompareTag("Fire"))
        {
            var fire = collision.gameObject.GetComponent<FireController>();
            if (fire)
            {
                if (!fire.fireExSE.isPlaying)
                {
                    fire.fireExSE.Play();
                }
            }
        }

        Destroy(gameObject, Settings.Bullet.LifetimeOnHit);
    }

    System.Collections.IEnumerator ExpandColliderTemporarily()
    {
        if (!sphereCollider) yield break;

        var originalRadius = sphereCollider.radius;
        sphereCollider.radius = originalRadius * Settings.Bullet.ImpactRadiusFactor;

        yield return new WaitForSeconds(Settings.Bullet.ImpactDuration);

        if (sphereCollider) sphereCollider.radius = originalRadius;
    }
}
