using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class RandomSizeOverLifetimeSetter : MonoBehaviour
{
    public int curveIndex = 0;
    [Header("SizeOverLifetimeにセットするカーブ配列")]
    public AnimationCurve[] curves;
    [Header("SizeOverLifetimeにセットするカーブ配列")]
    public float[] waterPitch;


    private int prevCurveIndex = -1;

    public AudioSource wateraudioSource;

    private void Start()
    {
        ApplyCurve();
        prevCurveIndex = curveIndex;
    }

    private void Update()
    {
        if (curveIndex != prevCurveIndex)
        {
            ApplyCurve();
            prevCurveIndex = curveIndex;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ApplyCurve();
        prevCurveIndex = curveIndex;
    }
#endif

    private void ApplyCurve()
    {
        if (curves == null || curves.Length == 0)
            return;

        int idx = Mathf.Clamp(curveIndex, 0, curves.Length - 1);

        var ps = GetComponent<ParticleSystem>();
        if (ps == null) return;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;

        ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve(1f, curves[idx]);
        sizeOverLifetime.size = curve;
        wateraudioSource.pitch = waterPitch[idx];
    }
}

