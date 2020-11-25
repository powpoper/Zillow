using System;
using System.IO;
using System.Text;
using System.Reflection;
using ConsoleApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp
{
    public sealed class Program
    {
        private static Uri BaseUri = new Uri("https://www.zillow.com");
        private static string Path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "DataDump.csv");
        public static void Main(string[] args)
        {
            var searchLink = new Uri("https://www.zillow.com/homes/77459_rb/");
            var zillowPages = new List<Page>();

            Console.WriteLine("Fetching Zillow Pages");

            do
            {
                var page = new Page(searchLink);
                zillowPages.Add(page);
                searchLink = new Uri(BaseUri.AbsoluteUri + page.NextPage);
            }
            while (!string.IsNullOrWhiteSpace(zillowPages.Last().NextPage));

            Console.WriteLine("Fetched {0} Zillow Pages", zillowPages.Count());

            var zillowSearchResults = zillowPages.SelectMany(x => x.SearchResults);

            Console.WriteLine("Creating CSV for {0} Zillow Search Results", zillowSearchResults.Count());

            var sb = new StringBuilder();            
            foreach (var sr in zillowSearchResults)
            {
                sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5}", sr.Status, sr.Address, sr.Price, sr.Price, sr.Link));
            }

            using (StreamWriter sw = File.CreateText(Path))
            {
                sw.WriteLine(sb.ToString());
            }

            Console.WriteLine("Finished.");
        }
    }
}
