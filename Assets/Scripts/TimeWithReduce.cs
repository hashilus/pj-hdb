using UnityEngine;

public class TimeWithReduce : MonoBehaviour
{
    public float reduceSpeed = 1f; // 1�b��1�����i�����j
    private float initialY;
    private bool isReducing = false;
    private Material targetMaterial;
    private Color originalColor;

    void Start()
    {
        // 0.5�`1.0�̃����_���ȃX�P�[���l�𐶐�
        float randomScale = Random.Range(0.5f, 1f);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        initialY = transform.localScale.y;
        // Renderer����}�e���A���擾
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            targetMaterial = renderer.material;
            originalColor = targetMaterial.color;
        }
        // 2�b��Ɍ����J�n
        StartCoroutine(StartReduceAfterDelay(2f));
    }

    private System.Collections.IEnumerator StartReduceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isReducing = true;
    }

    void Update()
    {
        if (!isReducing) return;

        Vector3 scale = transform.localScale;
        float newY = Mathf.MoveTowards(scale.y, 0f, reduceSpeed * Time.deltaTime);
        scale.y = newY;
        transform.localScale = scale;

        if (targetMaterial != null)
        {
            float yRatio = newY / initialY;
            if (yRatio < 0.5f)
            {
                // 0.5��0��alpha��1��0
                float alpha = Mathf.InverseLerp(0.5f, 0f, yRatio);
                float fadeValue = Mathf.Lerp(1f, 0f, alpha);

                targetMaterial.color = new Color(
                    originalColor.r, originalColor.g, originalColor.b,
                    fadeValue
                );
                // Smoothness�������l��
                targetMaterial.SetFloat("_Smoothness", fadeValue);
            }
        }

        if (newY <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
