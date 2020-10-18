using System.Collections;

static class CoroutineUtils
{
    public static IEnumerator Seq(params IEnumerator[] enumerators)
    {
        foreach (var enumerator in enumerators)
            yield return enumerator;
    }

    public static IEnumerator Par(params IEnumerator[] enumerators)
    {
        while (true)
        {
            bool cont = false;
            foreach (var enumerator in enumerators)
                cont |= enumerator.MoveNext();

            if (cont)
                yield return null;
            else
                yield break;
        }
    }
}
