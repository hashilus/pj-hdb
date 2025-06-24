using UnityEngine;

public class InstanceFromCollision : MonoBehaviour
{
    [Header("インスタンスするPrefab")]
    public GameObject prefabToInstantiate;

    private void OnCollisionEnter(Collision collision)
    {
//        if(Random.Range(0f,10f) > 2f) { 

            if (collision.gameObject.CompareTag("Water"))
            {
                Bullet bullet = collision.gameObject.GetComponent<Bullet>();
                if (bullet.hasInstanced)
                {
                    return; // すでに衝突していたら何もしない
                }
                bullet.hasInstanced = true; // 初回衝突としてマーク

                if (prefabToInstantiate != null)
                {
                    // 衝突位置
                    Vector3 position = collision.contacts[0].point;
                    // ランダムなY回転
                    float randomY = Random.Range(-180f, 180f);
                    Quaternion rotation = Quaternion.Euler(0f, randomY, 0f);
                    Instantiate(prefabToInstantiate, position, rotation);
                }
            }
        }

  //  }
}