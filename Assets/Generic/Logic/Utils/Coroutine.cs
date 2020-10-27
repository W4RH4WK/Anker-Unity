using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Timer
{
    public void Start(float duration)
    {
        StartTime = Time.time;
        EndTime = StartTime + duration;
    }

    public bool IsDone() => Time.time > EndTime;
    public IEnumerator Wait() => new WaitUntil(IsDone);

    public float Percent => Mathf.InverseLerp(StartTime, EndTime, Time.time);

    public float StartTime { get; private set; }
    public float EndTime { get; private set; } = 0.001f;
}

class OnOffAnimator
{
    public IEnumerator On(float duration = 0.0f) => SwitchTo(true, duration);
    public IEnumerator Off(float duration = 0.0f) => SwitchTo(false, duration);
    public IEnumerator SwitchTo(bool newState, float duration)
    {
        if (IsOn == newState)
            yield break;

        IsOn = newState;
        Timer.Start(duration);
        yield return Timer.Wait();
    }

    public float Percent
    {
        get {
            var percent = Timer.Percent;
            if (!IsOn)
                return 1 - percent;
            else
                return percent;
        }
    }

    public OnOffAnimator()
    {
        Timer = new Timer();
    }

    bool IsOn;
    Timer Timer;
}

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
