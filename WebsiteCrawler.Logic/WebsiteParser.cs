using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Logic.Models;
using System.Linq;

namespace WebsiteCrawler.Logic
{
    public class WebsiteParser
    {

        #region Private params
        int maxDeep;
        string baseUrl;
        PageParser pageParser;
        #endregion

        #region Public params
        public int TotalPagesParsed { get; set; }
        public Dictionary<string, int> DicAllInternalUrls { get; set; }
        #endregion

        #region Constructors
        public WebsiteParser(string BaseUrl, int MaxDeep = 100)
        {
            baseUrl = BaseUrl;
            maxDeep = MaxDeep;
            DicAllInternalUrls = new Dictionary<string, int>();
        } 
        #endregion

        public async Task Parse()
        {
            pageParser = new PageParser(baseUrl);
            pageParser.Page = new Page();

            await RecursiveParseInnerPages(baseUrl, 0, pageParser.Page);
        }

        private async Task RecursiveParseInnerPages(string Url, int Deep, Page Page)
        {
            if (Deep > maxDeep)
            {
                return;
            }

            TotalPagesParsed++;
            Console.WriteLine($"{TotalPagesParsed} - Parse: {Url}");            
            await pageParser.Parse(Url, Deep);

            if (pageParser.Page != null)
            {
                Page.InnerPages = GetOnlyNewInternalPages(pageParser.Page.InnerPages, Deep);

                for (int i = 0, length = Page.InnerPages.Count(); i < length; i++)
                {
                    var pageUrl = GetPageUrl(Page.InnerPages.ElementAt(i).Url);

                    await RecursiveParseInnerPages(pageUrl, Deep + 1, Page.InnerPages.ElementAt(i));
                }
            }            
        }

        private string GetPageUrl(string pageUrl)
        {
            if (!pageUrl.Contains(baseUrl))
            {
                pageUrl = $"{baseUrl}{pageUrl}";
            }

            return pageUrl;
        }

        private List<Page> GetOnlyNewInternalPages(List<Page> Pages, int Deep)
        {
            var newPages = new List<Page>();
            var pages = Pages.Where(x => x.IsExternal != true).ToList();            

            for (int i = 0, length = pages.Count(); i < length; i++)
            {
                var pageUrl = pages.ElementAt(i).Url;

                if (!pageUrl.Contains(baseUrl))
                {
                    pageUrl = $"{baseUrl}{pageUrl}";
                }

                if (!DicAllInternalUrls.ContainsKey(pageUrl) && !ImageHelper.IsImage(pageUrl))
                {
                    DicAllInternalUrls.Add(pageUrl, Deep);

                    newPages.Add(pages.ElementAt(i));
                }
            }

            return newPages;
        }

    }
}
