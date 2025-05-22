using UnityEngine;

public static class MonoBehaviourExtension
{
    public static T Clone<T>(this T original, bool active = true) where T : Component
    {
        var clone = Object.Instantiate(original);

        var cloneTr = clone.transform;
        var originalTr = original.transform;

        cloneTr.SetParent(originalTr.parent);
        cloneTr.localPosition = originalTr.localPosition;
        cloneTr.localRotation = originalTr.localRotation;
        cloneTr.localScale = originalTr.localScale;

        clone.gameObject.SetActive(active);

        return clone;
    }
}
