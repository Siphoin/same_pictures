using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SamePictures.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            System.Random random = new System.Random();

            return collection.OrderBy(x => random.Next());
        }

        public static T GetRandomElement<T>(this IEnumerable<T> source)
        {
            return source.ElementAt(Random.Range(0, source.Count()));
        }
    }
}
