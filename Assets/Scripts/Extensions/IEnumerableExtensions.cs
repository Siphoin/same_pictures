﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SamePictures.Extensions
{
    public static class IEnumerableExtensions
    {
        public static T GetRandomElement<T>(this IEnumerable<T> source)
        {
            List<T> elements = source.ToList();
            int randomIndex = Random.Range(0, elements.Count);
            return elements[randomIndex];
        }
    }
}