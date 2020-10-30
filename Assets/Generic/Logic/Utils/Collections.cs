using System.Linq;
using UnityEngine;

static class ArrayExtensions
{
    public static T RandomElement<T>(this T[] array) => array.ElementAtOrDefault(Random.Range(0, array.Length));
}
