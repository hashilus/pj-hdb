using UnityEngine;

public class DRigidPropAction : MonoBehaviour
{
    public float explosionForce = 500f;
    public Vector3 explosionOrigin;
    public Vector3 mainDirection = Vector3.up; // 主方向（例：上に吹き飛ぶ）

    private Rigidbody rigidbodies;

    public bool start;


    bool playOnce;

    
    public float torqueMagnitude = 200f; // Inspectorで調整可能に

    void Start()
    {
        // 子階層のRigidbodyを取得し、isKinematicをtrueにして物理停止
        rigidbodies = GetComponent<Rigidbody>();
        rigidbodies.isKinematic = true;
    }

    private void Update()
    {
        if (start && !playOnce) { 
            TriggerExplosion();
            playOnce = true;
        }
    }

    public void TriggerExplosion()
    {
        Rigidbody rb = rigidbodies;
        rb.isKinematic = false;
        // 爆破方向をベースに、少しだけランダムな方向を加える
        Vector3 toObj = (rb.worldCenterOfMass - explosionOrigin).normalized;
        Vector3 randomSpread = Random.insideUnitSphere * 0.3f; // 調整可能
        Vector3 finalDirection = (mainDirection + toObj * 0.5f + randomSpread).normalized;

        rb.AddForce(finalDirection * explosionForce, ForceMode.Impulse);

        // ランダムなトルクをかける
        Vector3 randomTorque = Random.onUnitSphere * torqueMagnitude;
        rb.AddTorque(randomTorque, ForceMode.Impulse);

        GetComponent<AudioSource>().Play();

        Destroy(gameObject, 3);
    }
}
