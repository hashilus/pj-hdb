using UnityEngine;
using System.Collections;

public class MaterialFader : MonoBehaviour
{
    public Material targetMaterial;    // �Ώۃ}�e���A��
    public Color fromColor = Color.clear;
    public Color toColor = Color.black;
    public float duration = 1f;

    private Coroutine fadeRoutine;

    public GameObject failedText; // Game Over���ɕ\������e�L�X�g

    private void Start()
    {
        failedText.SetActive(false); // ������Ԃł͔�\��
        targetMaterial.color = fromColor;

    }


    public void StartFade()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        float t = 0f;

        targetMaterial.color = fromColor;

        while (t < duration)
        {
            float progress = t / duration;
            targetMaterial.color = Color.Lerp(fromColor, toColor, progress);
            t += Time.deltaTime;
            yield return null;
        }

        failedText.SetActive(true); // Game Over���ɕ\������e�L�X�g��L����
        targetMaterial.color = toColor;
    }
}
