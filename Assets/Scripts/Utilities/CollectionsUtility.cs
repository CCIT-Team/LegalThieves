using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace New_Neo_LT.Scripts.Utilities
{
    public static class CollectionsUtility
    {
        public static IEnumerable<T> Grab<T>(this List<T> collection, int amount)
        {
            if (amount < 0 || amount > collection.Count) throw new System.IndexOutOfRangeException();
            if (amount == 0) return default;
            var grabBag = collection.Where(t => t != null).OrderBy(t => Random.value).ToList();
            if (amount > grabBag.Count) throw new System.IndexOutOfRangeException();
            return grabBag.Take(amount);
        }

        public static int RandomIndex<T>(this IList<T> list)
        {
            return Random.Range(0, list.Count);
        }

        public static T RandomElement<T>(this IList<T> list)
        {
            return list[RandomIndex(list)];
        }

        public static void ForEach<T>(this T[] arr, System.Action<T> action)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                action.Invoke(arr[i]);
            }
        }
    }
}