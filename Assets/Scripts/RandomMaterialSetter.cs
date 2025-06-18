using UnityEngine;

public class RandomMaterialSetter : MonoBehaviour
{
    [Header("ランダムでセットするマテリアル配列")]
    public Material[] materials;

    void Start()
    {
        if (materials != null && materials.Length > 0)
        {
            var renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                int idx = Random.Range(0, materials.Length);
                renderer.material = materials[idx];
            }
        }
    }
}
