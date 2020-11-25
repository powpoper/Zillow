﻿using System;
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
        private static readonly string BaseUri = "https://www.zillow.com";
        private static readonly string Path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "DataDump.csv");
        public static void Main(string[] args)
        {
            var searchLink = new Uri("https://www.zillow.com/homes/77459_rb/");
            var zillowPages = new List<Page>();

            Console.WriteLine("Fetching Zillow Pages");

            do
            {
                var page = new Page(searchLink);
                zillowPages.Add(page);
                searchLink = new Uri(BaseUri + page.NextPage);
            }
            while (!string.IsNullOrWhiteSpace(zillowPages.Last().NextPage) && zillowPages.Last().NextPage != zillowPages.Last().CurrentPageUrl.AbsolutePath);
            

            Console.WriteLine("Fetched {0} Zillow Pages", zillowPages.Count());

            var zillowSearchResults = zillowPages.SelectMany(x => x.SearchResults);

            Console.WriteLine("Creating CSV for {0} Zillow Search Results", zillowSearchResults.Count());

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
                    Console.WriteLine(ex.Message);
                }
                
            }

            using (StreamWriter sw = File.CreateText(Path))
            {
                sw.WriteLine(sb.ToString());
            }

            Console.WriteLine("Finished.");
        }
    }
}
