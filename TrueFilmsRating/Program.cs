using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
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
            var config = GetConfig();

            string trueFilmsUrl = config["trueFilmsUrl"];
            int numberOfThreads = Int32.Parse(config["numberOfThreads"]);
            string APIKey = config["IMDbApiKey"];
            string APIUrl = config["IMDbApiUrl"];

            if (args.Length > 0)
            {
                Console.Out.WriteLine("Getting API Key from arguments.");
                APIKey = args[0];
            }
            Console.Out.WriteLine($"TrueFilms URL: {trueFilmsUrl}, running on {numberOfThreads} threads.{rn}");


            Console.Out.WriteLine("Getting titles...");
            TrueFilmsScrapper tfScrapper = new TrueFilmsScrapper(trueFilmsUrl, numberOfThreads);
            var titlesCollections = await tfScrapper.GetTitlesAsync();
            Console.Out.WriteLine($"Found {titlesCollections.Sum(c=>c.Count())} titles.{rn}");

            Console.Out.Write("Getting ratings...");
            IMDbScrapper imdbScrapper = new IMDbScrapper(APIUrl, numberOfThreads, APIKey);

            List<List<string>> mockColl = new List<List<string>>();
            mockColl.Add(new List<string> { "sand", "stone" });
            mockColl.Add(new List<string> { "wind", "fire" });
            mockColl.Add(new List<string> { "water", "iron man" });

            var movies = await imdbScrapper.GetAllMoviesAsync(mockColl);

            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }

        #region private methods
        private static IConfigurationRoot GetConfig()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            return configuration;
        }
        #endregion
    }
}
