using UnityEngine;

public class DirectedExplosionController : MonoBehaviour
{
    public float explosionForce = 500f;
    public Vector3 explosionOrigin;
    public Vector3 mainDirection = Vector3.up; // ������i��F��ɐ�����ԁj

    private Rigidbody[] rigidbodies;

    public GameObject explosionParticle;


    public bool start;

    bool playOnce;

    void Start()
    {
        explosionParticle.SetActive(false);
        // �q�K�w��Rigidbody���擾���AisKinematic��true�ɂ��ĕ�����~
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
            playOnce = true; // ��x�������j���g���K�[
        }
    }

    public void TriggerExplosion()
    {
        explosionParticle.SetActive(true);
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = false;

            // ���j�������x�[�X�ɁA�������������_���ȕ�����������
            Vector3 toObj = (rb.worldCenterOfMass - explosionOrigin).normalized;
            Vector3 randomSpread = Random.insideUnitSphere * 0.3f; // �����\
            Vector3 finalDirection = (mainDirection + toObj * 0.5f + randomSpread).normalized;

            rb.AddForce(finalDirection * explosionForce, ForceMode.Impulse);
        }
    }
}
