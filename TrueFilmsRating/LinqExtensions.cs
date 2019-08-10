using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueFilmsRating
{
    static class LinqExtensions
    {
        /// <summary>
        /// Splits IEnumerable into equal parts
        /// </summary>
        /// <typeparam name="T">Type of items</typeparam>
        /// <param name="list">IEnumerable to split</param>
        /// <param name="parts">Number of parts</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            int i = 0;
            var splits = list.GroupBy(l => i++ % parts).AsEnumerable();

            return splits;
        }
    }
}
