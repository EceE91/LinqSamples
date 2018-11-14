using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            // func is an easy way to work with delegates
            // func kullanımındaki son generic type parametre her zaman return type'dır, 
            // sondan öncekilerse her zamanmethodun aldığı parametrelerdir
            // aşağıdaki func, takes an integer and returns an integer
            Func<int, int> square = x => x * x;

            // takes two integer, and return an integer
            Func<int, int, int> add = (x, y) => x + y;
            // add methodunu ayrıca aşağıdaki gibi de yazabiliriz, ikisi aynı şey
            Func<int, int, int> add2 = (x, y) =>
              {
                  int temp = x + y;
                  return temp;
              };

            Console.WriteLine(square(add(3, 5))); // 3+5 = 8, 8*8 = 64

            // Action, sadece incoming parametre alır ve return type yoktur, void döner
            // An Action type delegate is the same as Func delegate except that 
            // the Action delegate doesn't return a value. In other words, an Action delegate
            // can be used with a method that has a void return type.
            Action<int> write = x => Console.WriteLine(x); // console.writeline'ın return type'ı void olduğundan bu şekilde kullanım mümkündür
            write(square(add(3, 5))); // yukarıdaki ifade ile aynı sonucu(64) verir 


            IEnumerable<Employee> developers = new Employee[] {
                   new Employee { Id=  1, Name="Scott"},
                   new Employee{Id=2, Name="Ece"}
            };

            IEnumerable<Employee> sales = new List<Employee>() { new Employee { Id = 3, Name = "Özgün" } };

            Console.WriteLine(developers.MyCount());

            //IEnumerable Linq için önemli
            IEnumerator<Employee> enumerator = developers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine(enumerator.Current.Name);
            }


            // linq her zaman IEnumerable alır ve döndürür
            foreach (var employee in developers.Where(x => x.Name.Length == 5).OrderBy(x => x.Name))
            {
                Console.WriteLine(employee.Name);
            }

            // query syntax
            var queryStyntax = from developer in developers
                        where developer.Name.Length == 5
                        orderby developer descending
                        select developer;
            foreach (var item in queryStyntax)
            {
                Console.WriteLine(item.Name);
            }

            // method syntax
            var methodSyntax = developers.Where(x => x.Name.Length == 5).OrderByDescending(x => x.Name);

            foreach (var item in developers.Where(e => e.Name.StartsWith("S")))
            {
                Console.WriteLine(item.Name);
            }

            Console.ReadKey();            
        }
    }
}
