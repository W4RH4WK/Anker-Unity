using System.Collections;

static class CoroutineUtils
{
    public static IEnumerator par(params IEnumerator[] enumerators)
    {
        while (true)
        {
            bool cont = false;
            foreach (var enumerator in enumerators)
                cont |= enumerator.MoveNext();

            if (cont)
                yield return null;
            else
                break;
        }
    }
}
