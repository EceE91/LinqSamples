using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace LINQtoXML
{
    class Program
    {
        static void Main(string[] args)
        {
            //func
            Expression<Func<int, int, int>> add = (x, y) => x + y;
            Func<int, int, int> IAdd = add.Compile();

            var result = IAdd(3, 5);
            Console.WriteLine(result);
            Console.WriteLine(add);

            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());
            InsertData();
            QueryData();

            Console.ReadKey();
        }

        private static void InsertData()
        {
            var records = ProcessFile("fuel.csv");
            var db = new CarDb();

            db.Database.Log = Console.WriteLine;

            if (!db.Cars.Any())
            {
                foreach (var item in records)
                {
                    db.Cars.Add(item);
                }
                db.SaveChanges();
            }
        }

        private static void QueryData()
        {
            var db = new CarDb();

            // Entity framework ile kullanıldığında IQueryable tipinde oluyor, 
            // IQueryable takes an Expression method of Func. Boylece Entity framework kodu alıp
            // SQL statementa çeviriyor
            var query = from car in db.Cars
                        orderby car.Combined descending, car.Name ascending
                        select car;

            foreach (var item in query.Take(10))
            {
                Console.WriteLine($"{item.Combined}: {item.Name}");

            }

            // extension method syntax
            var query2 = db.Cars.Where(m => m.Name == "BMW").OrderByDescending(m => m.Combined).ThenBy(m => m.Name).Take(10).ToList();
        }

        private static List<Car> ProcessFile(string path)
        {
            var query2 = File.ReadAllLines(path).Skip(1).Where(l => l.Length > 1).ToCar();
            return query2.ToList();
        }
    } 

    public static class CarExtensions
    {
        // projection operator select gibi çalışıyor
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source) // string line al ve onu car'a dönüştür
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new Car // IEnumerable döndürmek için yield kullan, böylece deferred oldu
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3]),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7])
                };
            }
        }
    }
}
