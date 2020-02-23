using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Logic;
using System.Linq;
using WebsiteCrawler.Models.Requests;

namespace WebsiteCrawler.Console
{
    public static class OneThreadWebsiteParser
    {
        public static async Task Start()
        {
            var websiteParserRequest = new WebsiteParserRequest()
            {
                WebsiteUrl = "https://www.nytimes.com",
                MaxDeep = 2,
                DomainExtentions = null
            };

            var websiteParser = new WebsiteParser(websiteParserRequest);
            await websiteParser.Parse();

            await FileData.Save<object>("links.txt", websiteParser.DicAllInternalUrls.Select(x => new { url = x.Key, deep = x.Value }));
        }
    }
}
