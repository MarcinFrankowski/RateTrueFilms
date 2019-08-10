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

            DisplayMenu();
            bool keepGoing = true;
            while (keepGoing)
            {
                var key = Console.ReadKey(true).KeyChar;

                switch (key)
                {
                    case '1':
                        DisplayMovies(movies);
                        DisplayMenu();
                        break;
                    case '2':
                        SaveMovies(movies);
                        DisplayMenu();
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

        #region private methods
        private static void DisplayMenu()
        {
            Console.Out.Write($"{rn}What now? Choose an option by pressing a key:{rn}" +
                $"1 - Display movies ordered by rating.{rn}" +
                $"2 - Save movies data to movies.csv.{rn}" +
                $"x - Close.{rn}");
        }

        private static void DisplayMovies(IEnumerable<IMDbResponse> movies)
        {
            movies = movies.OrderByDescending(m => m.imdbRating);
            Console.Out.WriteLine($"{rn}{rn}List of movies by rating:");
            Console.Out.WriteLine($"Number \t Rating \t Title");

            int i = 1;
            foreach (var movie in movies)
            {
                Console.Out.WriteLine($"{i}. \t {movie.imdbRating}/10 \t {movie.Title} ({movie.Released})");
                i++;
            }
        }

        private static void SaveMovies(IEnumerable<IMDbResponse> movies)
        {
            throw new NotImplementedException();
        }

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
