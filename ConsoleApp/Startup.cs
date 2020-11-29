using ConsoleApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConsoleApp
{
    internal sealed class Startup
    {
        private readonly ILogger<Startup> _logger;
        private readonly IConfiguration _config;

        public Startup(ILogger<Startup> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public void Run()
        {
            var BaseUri = "https://www.zillow.com";
            var Path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "DataDump.csv");

            var searchLink = new Uri("https://www.zillow.com/homes/77459_rb/");
            var zillowPages = new List<Page>();
            
            _logger.LogInformation("Fetching Zillow Pages");

            do
            {
                var page = new Page(searchLink);
                zillowPages.Add(page);
                searchLink = new Uri(BaseUri + page.NextPage);
            }
            while (!string.IsNullOrWhiteSpace(zillowPages.Last().NextPage) && zillowPages.Last().NextPage != zillowPages.Last().CurrentPageUrl.AbsolutePath);


            _logger.LogInformation("Fetched {ZillowPageCount} Zillow Pages", zillowPages.Count());

            var zillowSearchResults = zillowPages.SelectMany(x => x.SearchResults);

            _logger.LogInformation("Creating CSV for {ZillowSearchResultsCount} Zillow Search Results", zillowSearchResults.Count());

            var sb = new StringBuilder();
            sb.AppendLine("Status,Address,Price,Link");
            foreach (var sr in zillowSearchResults)
            {
                try
                {
                    sb.AppendLine(string.Format("{0},{1},{2},{3}", sr.Status, sr.Address, sr.Price, sr.Link));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

            }

            using (StreamWriter sw = File.CreateText(Path))
            {
                sw.WriteLine(sb.ToString());
            }

            _logger.LogInformation("Finished.");
        }
    }
}
