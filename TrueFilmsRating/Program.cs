using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueFilmsRating.Scrappers;

namespace TrueFilmsRating
{
    class Program
    {
        private static string rn = Environment.NewLine;

        static async Task Main(string[] args)
        {
            string trueFilmsUrl = "http://truefilms.com";
            int numberOfThreads = 3;

            if (args.Length > 1)
            {
                Console.Out.WriteLine("Getting url and thread count from run arguments.");
                trueFilmsUrl = args[0];
                Int32.TryParse(args[1], out numberOfThreads);
            }
            Console.Out.WriteLine($"TrueFilms URL: {trueFilmsUrl}, running on {numberOfThreads} threads.{rn}");


            Console.Out.WriteLine("Getting titles...");
            TrueFilmsScrapper tfScrapper = new TrueFilmsScrapper(trueFilmsUrl, numberOfThreads);
            var titlesCollections = await tfScrapper.GetTitlesAsync();
            Console.Out.WriteLine($"Found {titlesCollections.Sum(c=>c.Count())} titles.{rn}");


            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }
    }
}
