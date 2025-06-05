using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    float lifetime = 2.0f;

    [SerializeField]
    float lifetimeOnHit = 1.0f;

    [SerializeField]
    GameObject hitParticlePrefab;

    [SerializeField]
    float hitParticleLifetime = 1.0f;

    [SerializeField, Range(0f, 1f)]
    float hitParticleSpawnChance = 0.5f;

    public PlayerID Shooter { get; set; }

    SphereCollider sphereCollider;
    bool hasCollided = false; // ← これが重要

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();

        Destroy(gameObject, lifetime);
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

        Destroy(gameObject, lifetimeOnHit);
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
