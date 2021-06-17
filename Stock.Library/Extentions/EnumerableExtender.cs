using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Library.Extentions
{
    /// <summary>
    /// Enumerable擴充
    /// </summary>
    public static class EnumerableExtender
    {
        /// <summary>
        /// IEnumerable Distinc泛型擴充方法
        /// </summary>
        /// <typeparam name="TSource">資料型別</typeparam>
        /// <typeparam name="TKey">選擇型別</typeparam>
        /// <param name="source">本身資料集合</param>
        /// <param name="keySelector">Func</param>
        /// <returns></returns>
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source is null)
            {
                yield return (TSource)source;
            }
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                var elementValue = keySelector(element);
                if (seenKeys.Add(elementValue))
                {
                    yield return element;
                }
            }
        }
    }
}
