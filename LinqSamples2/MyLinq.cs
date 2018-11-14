using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqSamples2
{
    public static class MyLinq
    {
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> source, Func<T,bool> predicate) {

            var result = new List<T>();

            foreach (var item in source) // source dediği movies
            {
                if (predicate(item)) // m => m.Year > 2000 ise
                {
                    // result.Add(item);
                    yield return item; // yield ifadesi ienumerable veya ienumerable<T> döndürür, deferred execution

                }
            }

            //return result;
        }
    }
}
