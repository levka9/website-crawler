using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Logic;
using System.Linq;

namespace WebsiteCrawler.Console
{
    public static class OneThreadWebsiteParser
    {
        public static async Task Start()
        {
            var websiteParser = new WebsiteParser("https://www.nytimes.com", 2);
            await websiteParser.Parse();

            await FileData.Save<object>("links.txt", websiteParser.DicAllInternalUrls.Select(x => new { url = x.Key, deep = x.Value }));
        }
    }
}
