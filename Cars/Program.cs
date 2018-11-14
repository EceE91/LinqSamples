using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {

            var cars = ProcessFile("fuel.csv");
            var manufacturers = ProcessManufacturersFile("manufacturers.csv");

            //// aşağıdaki queryde equals, inner join gibi çalışıyor. Yani eğer car.Manufacturer ile manufacturer.Name eşleşmiyorsa
            //// onu ekrana bastırmıyor
            //// note: bun LINQ/query syntax deniyor
            var query = from car in cars
                        join manufacturer in manufacturers on car.Manufacturer equals manufacturer.Name
                        orderby car.Combined descending, car.Name ascending
                        select new
                        {
                            manufacturer.Headquarters,
                            car.Manufacturer,
                            car.Combined
                        };

            //// extension syntax (sonunda select'i ayrıca yazmıyoruz)
            var query2 = cars.Join(manufacturers, c => c.Manufacturer, m => m.Name, (c, m) => new
            {
                m.Headquarters,
                c.Name,
                c.Manufacturer,
                c.Combined
            }).OrderByDescending(c => c.Combined).ThenBy(c => c.Name);


            foreach (var item in query2.Take(10))
            {
                Console.WriteLine($"{item.Headquarters} - {item.Manufacturer} - {item.Combined}");
            }

            //// eğer iki objeyi birden fazla yerden bağlacak olursak yani hem car.Manufacturer ile manufacturer.Name'i
            //// hem de car.Year ile manufacturer.Year'ı bağlamak istersek şöyle olur:
            var queryMultipleJoin = from car in cars
                                    join manufacturer in manufacturers on
                                    new { car.Manufacturer, car.Year } equals new { Manufacturer = manufacturer.Name, manufacturer.Year }
                                    orderby car.Combined descending, car.Name ascending
                                    select new
                                    {
                                        manufacturer.Headquarters,
                                        car.Manufacturer,
                                        car.Combined
                                    };
            //// yukarıda Manufacturer = manufacturer.Name deme sebebimiz equals'ın aynı olan propertyleri bağlama isteğinden gelmekte
            //// bu nedenle tıpkı  car.Year ve manufacturer.Year gibi olsun diye car.Manufacturer ile manufacturer.Name aynı olsun diye
            //// manufacturer.Name'i Manufacturer adlı bir değişkene atadık.

            //// Multiple join'in extension syntax ile yazılmış hali:
            var queryMultipleJoin2 = cars.Join(manufacturers,
                c => new { c.Manufacturer, c.Year },
                m => new { Manufacturer = m.Name, m.Year },
                (c, m) => new
                {
                    m.Headquarters,
                    c.Name,
                    c.Manufacturer,
                    c.Combined
                })
                            .OrderByDescending(c => c.Combined)
                            .ThenBy(c => c.Name);


            //// two most fuel efficient car by manufacturer ya da her bir manufacturer'a ait kaç adet car var
            var query3 = from car in cars
                         group car by car.Manufacturer;
            foreach (var group in query3)
            {
                Console.WriteLine($"{group.Key} has {group.Count()} cars"); // burada key dediğimiz şey, group by yaptığımız obje yani manufacturer

                // get two most fuel efficient car by manufacturer
                foreach (var car in group.OrderByDescending(m => m.Combined).Take(2))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }

            //// group into query syntax
            var query4 = from car in cars
                         group car by car.Manufacturer.ToUpper() into manufacturer
                         orderby manufacturer.Key // yani car.Manufacturer
                         select manufacturer;

            //// query4 extension method syntax
            var query4ExtensionMethod = cars.GroupBy(m => m.Manufacturer.ToUpper()).OrderBy(c => c.Key);

            ////groupjoin operation
            var queryGroupJoin = from manufacturer in manufacturers
                                 join car in cars on manufacturer.Name equals car.Manufacturer
                                 into carGroup
                                 select new
                                 {
                                     Manufacturer = manufacturer,
                                     Cars = carGroup
                                 };
            foreach (var group in queryGroupJoin)
            {
                Console.WriteLine($"{group.Manufacturer.Name} : {group.Manufacturer.Headquarters}");
                foreach (var item in group.Cars.OrderByDescending(c => c.Combined).Take(2))
                {
                    Console.WriteLine($"{item.Name}: {item.Combined}");
                }
            }

            // top three fuel efficient cars by country (Headquarters)        
            var query6_2 = from manufacturer in manufacturers
                                 join car in cars on manufacturer.Name equals car.Manufacturer
                                 into carGroup
                                 select new
                                 {
                                     Manufacturer = manufacturer,
                                     Cars = carGroup
                                 } into result
                                 group result by result.Manufacturer.Headquarters;

            foreach (var group in query6_2)
            {
                Console.WriteLine($"{group.Key}");
                foreach (var item in group.SelectMany(c => c.Cars).OrderByDescending(c=> c.Combined).Take(3))
                {
                    Console.WriteLine($"\t{item.Name}: {item.Combined}");
                }
            }

            // Aggregation
            var queryAggregation = from car in cars
                                   group car by car.Manufacturer
                                   into carGroup
                                   select new // projection
                                   {
                                       Name = carGroup.Key, // yani car.Manufacturer
                                       Max = carGroup.Max(c => c.Combined),
                                       Min = carGroup.Min(c => c.Combined),
                                       Avg = carGroup.Average(c => c.Combined)
                                   } into result
                                   orderby result.Max descending
                                   select result;

            foreach (var item in queryAggregation)
            {
                Console.WriteLine(item.Max);
            }


            //   QuerySamplesForCarcsv(cars);            

            Console.ReadKey();
        }

        private static void QuerySamplesForCarcsv(List<Car> cars)
        {
            // which car has the best fuel efficiency?
            var query = cars.OrderByDescending(m => m.Combined);
            // Diyelim en fuel efficient olanları bulduk ama bunların içinde fuel efficiency değeri 42 olan
            // 4 adet car var. Bunları da name'ine göre sort etmek istiyoruz. 
            // Bunun için cars.OrderByDescending(m => m.Combined).orderby(m=>m.name) dersek bu sefer bir önceki sortu
            // gölgeler ve sadece name'e göre sort eder. Biz bir secondary sort istiyoruz. Bunun için thenby kullanılır
            query = cars.OrderByDescending(m => m.Combined).ThenBy(m => m.Name);

            // aynı query'i farklı bir syntaxla şöyle yazarız
            var query2 = from car in cars orderby car.Combined descending, car.Name ascending select car;

            var query3 = from car in cars
                         where car.Manufacturer == "BMW" && car.Year == 2016
                         orderby car.Combined descending, car.Name ascending
                         select car;
            // note => "select" is LINQ's primary projection operator

            var isThereAny = query3.Any(); // is there anything in the dataset?
            var isThereAnyFordManifacturer = query3.Any(c => c.Manufacturer == "Ford"); // is there anything in the dataset that has a Ford manufacturer?
            Console.WriteLine(isThereAnyFordManifacturer);

            var areAllCarsManufacturedByFord = query3.All(c => c.Manufacturer == "Ford"); // do all of the cars have a manufacturer of Ford?
            Console.WriteLine(areAllCarsManufacturedByFord);


            var top = cars.Where(m => m.Manufacturer == "BMW" && m.Year == 2016)
                .OrderByDescending(m => m.Combined).ThenBy(m => m.Name).Select(c => c).First(); // sonuncuyu almak istersek last() kullanılır.
            Console.Write(top.Name);

            foreach (var item in query.Take(10))
            {
                Console.WriteLine($"{item.Manufacturer} {item.Name}:{item.Combined}");
            }

            // projection sayesinde sadece gerekli kolonları çektik, daha efficient
            var queryWithProjection = from car in cars
                                      where car.Manufacturer == "BMW" && car.Year == 2016
                                      orderby car.Combined descending, car.Name ascending
                                      select new { car.Manufacturer, car.Name, car.Combined };
            // ya da 
            var queryWithProjection2 = cars.Where(m => m.Manufacturer == "BMW" && m.Year == 2016)
                .OrderByDescending(m => m.Combined).ThenBy(m => m.Name).Select(c => new { c.Name, c.Manufacturer, c.Combined });



            // selectmany operator da bir projection operator'dır ve flattening operator olarak da bilinir. Because
            // it flattens a collection of collections into a single collection
            var result = cars.SelectMany(c => c.Name); // pek kullanıcağın birşey değil, name'in characterlerini getirir 
            foreach (var item in result)
            {
                Console.WriteLine(item);
            }


            //foreach (var item in cars)
            //{
            //    Console.WriteLine(item.Name);
            //}
        }

        private static List<Manufacturer> ProcessManufacturersFile(string path)
        {
            var query = File.ReadAllLines(path).Where(l => l.Length > 1).Select(l =>
            {
                var columns = l.Split(',');
                return new Manufacturer
                {
                    Name = columns[0],
                    Headquarters = columns[1],
                    Year = int.Parse(columns[2])
                };
            });

            return query.ToList();
        }

        private static List<Car> ProcessFile(string path)
        {
            //return
            //    File.ReadAllLines(path).Skip(1) // header'ı skip et
            //    .Where(line => line.Length > 1) // boş satırları alma
            //    .Select(Car.ParseFromCSV) // csv'den oku ve sonrasında Car objesine maple
            //    .ToList();

            //var query = from line in File.ReadAllLines(path).Skip(1)
            //            where line.Length > 1
            //            select Car.ParseFromCSV(line);

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
                yield return new  Car // IEnumerable döndürmek için yield kullan, böylece deferred oldu
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
