using UnityEngine;

public class ProjectileAutoDestroy : MonoBehaviour
{
    public GameObject particlePrefab;
    [Range(0f, 1f)]
    public float spawnChance = 0.5f;

    private SphereCollider sphereCol;
    private ReticleController reticle;

    public ReticleController Reticle { set => reticle = value; }

    private float originalRadius;
    private bool hasCollided = false; // ← これが重要

    void Start()
    {
        sphereCol = GetComponent<SphereCollider>();
        if (sphereCol != null)
            originalRadius = sphereCol.radius;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return; // すでに衝突していたら何もしない

        hasCollided = true; // 初回衝突としてマーク

        // パーティクル生成（確率）
        if (particlePrefab != null && Random.value <= spawnChance)
        {
            GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            Destroy(particle, 1f);
        }

        // レティクル表示（初回のみ）
        if (reticle != null)
            reticle.ShowAt(transform.position);

        // 拡大コライダー処理
        if (sphereCol != null)
        {
            StartCoroutine(ExpandColliderTemporarily());
        }

        Destroy(gameObject, 1f);
    }

    System.Collections.IEnumerator ExpandColliderTemporarily()
    {
        sphereCol.radius = originalRadius * Settings.Bullet.ImpactRadiusFactor;
        yield return new WaitForSeconds(Settings.Bullet.ImpactDuration);
        if (sphereCol != null)
            sphereCol.radius = originalRadius;
    }
}
