using UnityEngine;

public static class SingletonComponentCreator
{
    public static T Create<T>(bool dontDestroyOnLoad = false) where T : Component
    {
        var go = new GameObject(typeof(T).Name);
        if (dontDestroyOnLoad)
        {
            Object.DontDestroyOnLoad(go);
        }
        return go.AddComponent<T>();
    }

    public static T CreateAndAssignIfNull<T>(ref T variable, bool dontDestroyOnLoad = false) where T : Component
    {
        if (variable != null) return variable;
        variable = Create<T>(dontDestroyOnLoad);
        return variable;
    }
}
