using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    static CoroutineRunner instance;
    static CoroutineRunner Instance { get { return SingletonComponentCreator.CreateAndAssignIfNull(ref instance, true); } }

    public static Coroutine Run(IEnumerator routine)
    {
        return Instance.StartCoroutine(routine);
    }
}
