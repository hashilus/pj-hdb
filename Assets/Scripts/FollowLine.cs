using UnityEngine;

public class FollowLine : MonoBehaviour
{
    private Transform from;
    private Transform to;
    private LineRenderer line;

    private float startWidth;
    private float maxWidth;
    private float growthSpeed;

    public void Init(Transform a, Transform b, float startW, float maxW, float speed)
    {
        from = a;
        to = b;
        startWidth = startW;
        maxWidth = maxW;
        growthSpeed = speed;
        line = GetComponent<LineRenderer>();
        line.startWidth = startWidth;
        line.endWidth = startWidth;
    }

    void Update()
    {
        if (from != null && to != null)
        {
            line.SetPosition(0, from.position);
            line.SetPosition(1, to.position);
        }

        // ëæÇ≥Çéûä‘Ç≈ëùâ¡Ç≥ÇπÇÈ
        float newWidth = Mathf.Min(line.startWidth + growthSpeed * Time.deltaTime, maxWidth);
        line.startWidth = newWidth;
        line.endWidth = newWidth;
    }
}
