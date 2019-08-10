using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueFilmsRating.Scrappers
{
    public  class TrueFilmsScrapper
    {
        private readonly string _trueFilmsUrl;
        private readonly int _numberOfParts;

        public TrueFilmsScrapper(string trueFilmsUrl, int numberOfParts)
        {
            _trueFilmsUrl = trueFilmsUrl;
            _numberOfParts = numberOfParts;
        }

        public async Task<IEnumerable<IEnumerable<string>>> GetTitlesAsync()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = await web.LoadFromWebAsync(_trueFilmsUrl);

            var listItems = doc.DocumentNode.SelectNodes("//*[@id=\"catnavlist\"]/ul[2]/li/a");

            var titles = listItems.Select(li => li.InnerHtml);
            return titles.Split(_numberOfParts);
        }
    }
}
