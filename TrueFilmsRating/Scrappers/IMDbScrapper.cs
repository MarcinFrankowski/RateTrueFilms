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

        public async Task<IMDbResponse> GetMovie(string title, int flurlClientIndex = 0)
        {
            return await _flurlClients[flurlClientIndex].Request()
                .SetQueryParams(new { apikey = _APIKey, t = title })
                .WithTimeout(10)
                .GetJsonAsync<IMDbResponse>();
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
