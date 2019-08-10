using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrueFilmsRating.Models;
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
            Console.Out.WriteLine($"Found {titlesCollections.Sum(c => c.Count())} titles.{rn}");

            Console.Out.Write($"Getting ratings...{rn}");
            IMDbScrapper imdbScrapper = new IMDbScrapper(APIUrl, numberOfThreads, APIKey);

            List<List<string>> mockColl = new List<List<string>>();
            mockColl.Add(new List<string> { "sand", "stone" });
            mockColl.Add(new List<string> { "wind", "fire" });
            mockColl.Add(new List<string> { "water", "iron man" });

            var movies = await imdbScrapper.GetAllMoviesAsync(mockColl);
            Console.Out.Write($"Found ratings for {movies.Count()} movies.{rn}{rn}");

            bool keepGoing = true;
            while (keepGoing)
            {
                Console.Out.Write($"What now? Choose an option by pressing a key:{rn}" +
                    $"1 - Display movies ordered by rating.{rn}" +
                    $"2 - Save movies data to movies.csv.{rn}" +
                    $"x - Close.{rn}");

                var key = Console.ReadKey().KeyChar;

                switch (key)
                {
                    case '1':
                        DisplayMovies(movies);
                        break;
                    case '2':
                        SaveMovies(movies);
                        break;
                    case 'X':
                    case 'x':
                        keepGoing = false;
                        break;
                    default:
                        break;
                }
            }

        }

        private static void DisplayMovies(IEnumerable<IMDbResponse> movies)
        {
            throw new NotImplementedException();
        }

        private static void SaveMovies(IEnumerable<IMDbResponse> movies)
        {
            throw new NotImplementedException();
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
