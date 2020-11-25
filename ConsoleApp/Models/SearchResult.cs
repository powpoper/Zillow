using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Models
{
    public sealed class SearchResult
    {
        private HtmlNode _htmlNode;
        public SearchResult(HtmlNode htmlNode)
        {
            _htmlNode = htmlNode;
        }

        public string Status => _htmlNode?.QuerySelector("div.list-card-type")?.InnerText;
        public string Price => _htmlNode?.QuerySelector("div.list-card-price")?.InnerText.Replace(",", string.Empty);
        public string Address => _htmlNode?.QuerySelector("div.list-card-info address")?.InnerText.Replace(",", string.Empty);
        public string Link => _htmlNode?.QuerySelector("div.list-card-info a")?.GetAttributeValue("href", null);
    }
}
