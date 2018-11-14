using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqSamples2
{
    class Program
    {
        static void Main(string[] args)
        {
            // linq query syntax starts with "from"
            // and ends with "select" or "group"
            string[] cities = new string[] { "istanbul", "izmir", "london"};

            IEnumerable<string> filteredCities = from city in cities
                                                 where city.StartsWith("l") && city.Length < 15
                                                 orderby city // default asc
                                                 select city;


            var movies = new List<Movie>
            {
                new Movie { Title ="The Movie 1", Rating = 0.8f, Year=2016 },
                new Movie {Title ="The Movie 2", Rating = 0.3f, Year=2017},
                new Movie {Title ="C Movie 3", Rating = 0.7f, Year=1897},
                new Movie {Title ="Star wars", Rating = 0.3f, Year=1997},
            };

            var query = movies.Where(m => m.Year > 2000);
            var query2 = movies.Filter(m => m.Year > 2000); // movies burada source yani (this IEnumerable<T> source)
            // m => m.Year > 2000 ifadesi de Func<T,bool> predicate methodundaki T'ye denk geliyor
            foreach (var item in query2)
            {
                Console.WriteLine(item.Title);
            }

            Console.WriteLine(query2.Count());

            var enumerator = query2.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine(enumerator.Current.Title);
            }

            Console.ReadKey();
        }
    }
}
