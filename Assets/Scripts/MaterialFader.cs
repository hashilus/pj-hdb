using UnityEngine;
using System.Collections;

public class MaterialFader : MonoBehaviour
{
    public Material targetMaterial;    // 対象マテリアル
    public Color fromColor = Color.clear;
    public Color toColor = Color.black;
    public float duration = 1f;

    private Coroutine fadeRoutine;

    public GameObject failedText; // Game Over時に表示するテキスト

    private void Start()
    {
        failedText.SetActive(false); // 初期状態では非表示
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

        failedText.SetActive(true); // Game Over時に表示するテキストを有効化
        targetMaterial.color = toColor;
    }
}
