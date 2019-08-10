using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueFilmsRating.Scrappers
{
    public  class IMDbScrapper
    {
        private readonly string _IMDbSearchUrl;
        private readonly int _numberOfThreads;

        public IMDbScrapper(string IMDbSearchUrl, int numberOfThreads)
        {
            _IMDbSearchUrl = IMDbSearchUrl;
            _numberOfThreads = numberOfThreads;
        }
    }
}
