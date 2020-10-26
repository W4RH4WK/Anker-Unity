using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Unity's coroutine API is provided through MonoBehaviour. We therefore add the
// following helper functions as extensions method to MonoBehaviour.
static class MonoBehaviourExtensions
{
    public static IEnumerator Par(this MonoBehaviour monoBehaviour, params IEnumerator[] coroutines)
    {
        var startedCoroutines = new List<Coroutine>();

        foreach (var coroutine in coroutines)
            startedCoroutines.Add(monoBehaviour.StartCoroutine(coroutine));

        foreach (var coroutine in startedCoroutines)
            yield return coroutine;
    }
}
