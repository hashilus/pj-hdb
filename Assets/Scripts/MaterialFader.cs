using UnityEngine;
using System.Collections;

public class MaterialFader : MonoBehaviour
{
    public Material targetMaterial;
    public Color fromColor = Color.clear;
    public Color toColor = Color.black;
    public float duration = 1f;


    public bool fadeIn = false;   // Timelineから直接制御
    public bool fadeOut = false;  // Timelineから直接制御

    private bool prevFadeIn = false;
    private bool prevFadeOut = false;

    private Coroutine fadeRoutine;
    public GameObject failedText;

    private void Start()
    {
        if (targetMaterial == null)
        {
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                targetMaterial = renderer.material;
            }
        }

        if (failedText != null)
        {
            failedText.SetActive(false);
        }
        if (targetMaterial != null)
        {
            targetMaterial.color = fromColor;
        }

    }

    private void Update()
    {
        // fadeInがFalse→Trueになった瞬間だけ発火
        if (!prevFadeIn && fadeIn)
        {
            FadeIn();
        }
        // fadeOutがFalse→Trueになった瞬間だけ発火
        if (!prevFadeOut && fadeOut)
        {
            FadeOut();
        }
        prevFadeIn = fadeIn;
        prevFadeOut = fadeOut;
    }

    public void FadeIn()
    {
        StartFade(fromColor, toColor);
    }

    public void FadeOut()
    {
        StartFade(toColor, fromColor);
    }

    private void StartFade(Color start, Color end)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(start, end));
    }

    private IEnumerator FadeRoutine(Color start, Color end)
    {
        float t = 0f;
        if (targetMaterial == null)
            yield break;

        targetMaterial.color = start;

        while (t < duration)
        {
            float progress = t / duration;
            targetMaterial.color = Color.Lerp(start, end, progress);
            t += Time.deltaTime;
            yield return null;
        }
        targetMaterial.color = end;

        if (failedText != null && end == toColor)
        {
            failedText.SetActive(true);
        }
    }
}
