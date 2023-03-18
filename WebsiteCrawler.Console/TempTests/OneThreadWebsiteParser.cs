using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Logic;
using System.Linq;
using WebsiteCrawler.Models.Requests;
using System.Collections.Concurrent;
using WebsiteCrawler.Logic.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace WebsiteCrawler.Console.TempTests
{
    public static class OneThreadWebsiteParser
    {
        public static async Task Start(ServiceProvider serviceProvider)
        {
            var websiteParserModuleRequest = new WebsiteParserModuleRequest()
            {
                TaskId = 1,
                DomainName = "2net.co.il",
                DomainLevel = Model.Enums.EDomainLevel.SecondLevel,
                WebsiteParserLimitsRequest = new WebsiteParserLimitsRequest()
                {
                    MaxDeep = 2,
                    MaxInternalLinks = 20,
                    
                },
                DomainExtentions = new List<string>()
                {
                    "co.il",
                    "org.il"
                }
            };

            WebSitesConcurrentQueue.WebSites = new ConcurrentQueue<string>();
            WebSitesConcurrentQueue.AllWebSites = new ConcurrentQueue<string>();

            var websiteParserModule = serviceProvider.GetService<IWebsiteParserModule>();

            await websiteParserModule.ParseAsync(websiteParserModuleRequest);

            System.Console.WriteLine("Finished");
            // using (var websiteParser = new WebsiteParserModule(websiteParserModuleRequest, pageDataParserModule))
            // {
            //     await websiteParser.Parse();

            //     await FileData.SerializeAndSaveAsync<IEnumerable<object>>("links.txt", websiteParser.DicAllInternalUrls.Select(x => new { url = x.Key, deep = x.Value }));
            // }
        }
    }
}
