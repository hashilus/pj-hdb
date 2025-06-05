using UnityEngine;

public class DirectedExplosionController : MonoBehaviour
{
    public float explosionForce = 500f;
    public Vector3 explosionOrigin;
    public Vector3 mainDirection = Vector3.up; // 主方向（例：上に吹き飛ぶ）

    private Rigidbody[] rigidbodies;

    public GameObject explosionParticle;


    public bool start;

    bool playOnce;

    void Start()
    {
        explosionParticle.SetActive(false);
        // 子階層のRigidbodyを取得し、isKinematicをtrueにして物理停止
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        if (start && !playOnce)
        {
            TriggerExplosion();
            start = false;
            playOnce = true; // 一度だけ爆破をトリガー
        }
    }

    public void TriggerExplosion()
    {
        explosionParticle.SetActive(true);
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = false;

            // 爆破方向をベースに、少しだけランダムな方向を加える
            Vector3 toObj = (rb.worldCenterOfMass - explosionOrigin).normalized;
            Vector3 randomSpread = Random.insideUnitSphere * 0.3f; // 調整可能
            Vector3 finalDirection = (mainDirection + toObj * 0.5f + randomSpread).normalized;

            rb.AddForce(finalDirection * explosionForce, ForceMode.Impulse);
        }
    }
}
