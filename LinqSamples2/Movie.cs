using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqSamples2
{
   public class Movie
    {
        public string Title { get; set; }
        public float Rating { get; set; }
        //public int Year { get; set; }

        int _year; // private by default
        public int Year
        {
            get
            {
                Console.WriteLine($"Returning {_year} for {Title}");
                return _year;
            }
            set { _year = value; }
        }
    }
}
