using System.Collections;
using System.Linq;

static class CoroutineUtils
{
    public static IEnumerator Seq(params IEnumerator[] enumerators)
    {
        foreach (var enumerator in enumerators)
            yield return enumerator;
    }

    public static IEnumerator Par(params IEnumerator[] enumerators)
    {
        // Cannot use Any where due to short-circuit evaluation.
        while (enumerators.Count(e => e.MoveNext()) > 0)
            yield return null;
    }
}
