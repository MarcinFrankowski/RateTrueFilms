using CsvHelper;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
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

            var movies = await imdbScrapper.GetAllMoviesAsync(titlesCollections);
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
                        SaveMoviesToCSV(movies);
                        Console.Out.WriteLine($"{rn}Movies saved in truefilms.csv");
                        DisplayMenu();
                        break;
                    case '3':
                        SaveMoviesToXLSX(movies);
                        Console.Out.WriteLine($"{rn}Movies saved in truefilms.xlsx");
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
                $"2 - Save movies data to CSV file (truefilms.csv).{rn}" +
                $"3 - Save movies data to Excel file (truefilms.xlsx).{rn}" +
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

        private static void SaveMoviesToCSV(IEnumerable<IMDbResponse> movies)
        {
            using (var writer = new StreamWriter("truefilms.csv"))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(movies);
            }
        }

        private static void SaveMoviesToXLSX(IEnumerable<IMDbResponse> movies)
        {
            List<string> colNames = new List<string>();
            foreach (PropertyInfo prop in typeof(IMDbResponse).GetProperties())
            {
                colNames.Add(prop.Name);
            }

            using (FileStream stream = new FileStream("truefilms.xlsx", FileMode.Create))
            {
                IWorkbook wb = new XSSFWorkbook();
                ISheet sheet = wb.CreateSheet("TrueFilms");
                ICreationHelper cH = wb.GetCreationHelper();

                IRow topRow = sheet.CreateRow(0);
                int ic = 0;
                foreach (var col in colNames)
                {
                    ICell cell = topRow.CreateCell(ic);
                    cell.SetCellValue(col);
                    ic++;
                }

                int im = 1;
                foreach (var movie in movies)
                {
                    IRow row = sheet.CreateRow(im);
                    for (int j = 0; j < colNames.Count; j++)
                    {
                        ICell cell = row.CreateCell(j);
                        cell.SetCellValue(typeof(IMDbResponse).GetProperties().Single(p => string.Equals(p.Name, colNames[j])).GetValue(movie).ToString());
                    }
                    im++;
                }
                wb.Write(stream);
            }
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
