using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace ConsoleApp.Models
{
    public sealed class Page
    {
        private HtmlDocument _htmlDoc;

        public Page(Uri searchLink)
        {
            HtmlWeb web = new HtmlWeb();
            _htmlDoc = web.Load(searchLink);
            CurrentPageUrl = searchLink;
        }

        public Uri CurrentPageUrl;
        public IEnumerable<SearchResult> SearchResults =>
                    _htmlDoc.QuerySelectorAll("article.list-card")
                    .Select(node => new SearchResult(node));
        public string NextPage 
        {
            get 
            {
                var links = _htmlDoc.QuerySelectorAll("div.search-pagination ul li a");

                return links.Count() > 0 ? links.Last().GetAttributeValue("href", null) : null;
            }
        }
    }
}
