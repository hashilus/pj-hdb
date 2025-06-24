using UnityEngine;

public class DRigidPropAction : MonoBehaviour
{
    public float explosionForce = 500f;
    public Vector3 explosionOrigin;
    public Vector3 mainDirection = Vector3.up; // ������i��F��ɐ�����ԁj

    private Rigidbody rigidbodies;

    public bool start;


    bool playOnce;

    
    public float torqueMagnitude = 200f; // Inspector�Œ����\��

    void Start()
    {
        // �q�K�w��Rigidbody���擾���AisKinematic��true�ɂ��ĕ�����~
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
        // ���j�������x�[�X�ɁA�������������_���ȕ�����������
        Vector3 toObj = (rb.worldCenterOfMass - explosionOrigin).normalized;
        Vector3 randomSpread = Random.insideUnitSphere * 0.3f; // �����\
        Vector3 finalDirection = (mainDirection + toObj * 0.5f + randomSpread).normalized;

        rb.AddForce(finalDirection * explosionForce, ForceMode.Impulse);

        // �����_���ȃg���N��������
        Vector3 randomTorque = Random.onUnitSphere * torqueMagnitude;
        rb.AddTorque(randomTorque, ForceMode.Impulse);

        GetComponent<AudioSource>().Play();

        Destroy(gameObject, 3);
    }
}
