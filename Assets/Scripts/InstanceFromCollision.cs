using UnityEngine;

public class InstanceFromCollision : MonoBehaviour
{
    [Header("�C���X�^���X����Prefab")]
    public GameObject prefabToInstantiate;

    private void OnCollisionEnter(Collision collision)
    {
//        if(Random.Range(0f,10f) > 2f) { 

            if (collision.gameObject.CompareTag("Water"))
            {
                Bullet bullet = collision.gameObject.GetComponent<Bullet>();
                if (bullet.hasInstanced)
                {
                    return; // ���łɏՓ˂��Ă����牽�����Ȃ�
                }
                bullet.hasInstanced = true; // ����Փ˂Ƃ��ă}�[�N

                if (prefabToInstantiate != null)
                {
                    // �Փˈʒu
                    Vector3 position = collision.contacts[0].point;
                    // �����_����Y��]
                    float randomY = Random.Range(-180f, 180f);
                    Quaternion rotation = Quaternion.Euler(0f, randomY, 0f);
                    Instantiate(prefabToInstantiate, position, rotation);
                }
            }
        }

  //  }
}