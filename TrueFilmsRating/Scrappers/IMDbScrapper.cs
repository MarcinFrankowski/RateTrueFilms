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
    public  class IMDbScrapper
    {
        private readonly string _IMDbSearchUrl;
        private readonly int _numberOfThreads;
        private readonly string _APIKey;

        public IMDbScrapper(string IMDbSearchUrl, int numberOfThreads,string APIKey)
        {
            _IMDbSearchUrl = IMDbSearchUrl;
            _numberOfThreads = numberOfThreads;
            _APIKey = APIKey;
        }

        public async Task<IMDbResponse> GetMovie(string title)
        {
            // Flurl will use 1 HttpClient instance per host
            return await _IMDbSearchUrl
                .SetQueryParams(new { apikey = _APIKey, t = title })
                .WithTimeout(10)
                .GetJsonAsync<IMDbResponse>();
        }
    }

    
}
