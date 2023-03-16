using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Logic;
using System.Linq;
using WebsiteCrawler.Models.Requests;
using System.Collections.Concurrent;
using WebsiteCrawler.Logic.Modules.Interfaces;

namespace WebsiteCrawler.Console.TempTests
{
    public static class OneThreadWebsiteParser
    {
        public static async Task Start(IPageDataParserModule pageDataParserModule)
        {
            var websiteParserModuleRequest = new WebsiteParserModuleRequest()
            {
                DomainName = "www.sport5.co.il",
                WebsiteParserLimitsRequest = new WebsiteParserLimitsRequest()
                {
                    MaxDeep = 2,
                    MaxInternalLinks = 20
                },
                DomainExtentions = new List<string>()
                {
                    "co.il",
                    "org.il"
                }
            };

            // WebSitesConcurrentQueue.WebSites = new ConcurrentQueue<string>();
            // WebSitesConcurrentQueue.AllWebSites = new ConcurrentQueue<string>();

            // using (var websiteParser = new WebsiteParserModule(websiteParserModuleRequest, pageDataParserModule))
            // {
            //     await websiteParser.Parse();

            //     await FileData.SerializeAndSaveAsync<IEnumerable<object>>("links.txt", websiteParser.DicAllInternalUrls.Select(x => new { url = x.Key, deep = x.Value }));
            // }
        }
    }
}
