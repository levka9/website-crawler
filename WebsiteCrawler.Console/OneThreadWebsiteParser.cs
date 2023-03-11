using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Logic;
using System.Linq;
using WebsiteCrawler.Models.Requests;
using System.Collections.Concurrent;

namespace WebsiteCrawler.Console
{
    public static class OneThreadWebsiteParser
    {
        public static async Task Start()
        {
            var websiteParserRequest = new WebsiteParserRequest()
            {
                DomainName = "www.sport5.co.il",
                MaxDeep = 2,
                DomainExtentions = new List<string>()
                {
                    "co.il",
                    "org.il"
                }
            };

            WebSitesConcurrentQueue.WebSites = new ConcurrentQueue<string>();
            WebSitesConcurrentQueue.AllWebSites = new ConcurrentQueue<string>();            

            using (var websiteParser = new WebsiteParser(websiteParserRequest))
            {
                await websiteParser.Parse();

                await FileData.SerializeAndSaveAsync<IEnumerable<object>>("links.txt", websiteParser.DicAllInternalUrls.Select(x => new { url = x.Key, deep = x.Value }));
            }            
        }
    }
}
