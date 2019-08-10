using Flurl;
using Flurl.Http;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueFilmsRating.Models;

namespace TrueFilmsRating.Scrappers
{
    public class IMDbScrapper : IDisposable
    {
        private readonly string _IMDbSearchUrl;
        private readonly int _numberOfThreads;
        private readonly string _APIKey;

        List<FlurlClient> _flurlClients;

        public IMDbScrapper(string IMDbSearchUrl, int numberOfThreads, string APIKey)
        {
            _IMDbSearchUrl = IMDbSearchUrl;
            _numberOfThreads = numberOfThreads > 0 ? numberOfThreads : 1;
            _APIKey = APIKey;

            _flurlClients = new List<FlurlClient>();
            for (int i = 0; i < _numberOfThreads; i++)
            {
                _flurlClients.Add(new FlurlClient(_IMDbSearchUrl));
            }
        }

        public async Task<IEnumerable<IMDbResponse>> GetAllMoviesAsync(IEnumerable<IEnumerable<string>> titlesCollections)
        {
            List<Task<IEnumerable<IMDbResponse>>> getTasks = new List<Task<IEnumerable<IMDbResponse>>>();
            int i = 0;
            foreach (var titles in titlesCollections)
            {
                getTasks.Add(GetMoviesAsync(titles, i));
                i++;
            }

            var moviesCollections = await Task.WhenAll(getTasks);

            // Flatten collection
            var movies = moviesCollections.SelectMany(collection => collection).Distinct();

            return movies;
        }

        public async Task<IEnumerable<IMDbResponse>> GetMoviesAsync(IEnumerable<string> titles, int flurlClientIndex = 0)
        {
            var movies = new List<IMDbResponse>();
            foreach (var title in titles)
            {
                var movie = await this.GetMovie(title, flurlClientIndex);
                if (string.Equals(movie.Response, "True"))
                {
                    movies.Add(movie);
                }
                else
                {
                    Console.Out.WriteLine($"Failed to find movie {title}.");
                }
            }
            return movies;
        }

        public async Task<IMDbResponse> GetMovie(string title, int flurlClientIndex = 0)
        {
            try
            {
                return await _flurlClients[flurlClientIndex].Request()
                .SetQueryParams(new { apikey = _APIKey, t = title })
                .WithTimeout(10)
                .GetJsonAsync<IMDbResponse>();
            }
            catch (Exception)
            {
                return new IMDbResponse();
            }
            
        }

        public void Dispose()
        {
            foreach (var client in _flurlClients)
            {
                client.Dispose();
            }
        }

    }


}
