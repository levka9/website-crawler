using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Models;
using System.Linq;
using WebsiteCrawler.Models.Requests;
using System.Threading;

namespace WebsiteCrawler.Logic
{
    public class WebsiteParser
    {

        #region Private params
        int maxDeep;
        string domain;
        string baseUrl;        
        PageParser pageParser;
        IEnumerable<string> domainExtentions;
        int? taskId;
        #endregion

        #region Public params
        public int TotalPagesParsed { get; set; }
        public Dictionary<string, int> DicAllInternalUrls { get; set; }
        #endregion

        #region Constructors
        public WebsiteParser(WebsiteParserRequest WebsiteParserRequest)
        {
            taskId = WebsiteParserRequest.TaskId;
            baseUrl = WebsiteParserRequest.WebsiteUrl;
            maxDeep = WebsiteParserRequest.MaxDeep;
            domainExtentions = WebsiteParserRequest.DomainExtentions;
            domain = Url.GetDomain(WebsiteParserRequest.WebsiteUrl);

            DicAllInternalUrls = new Dictionary<string, int>();
        } 
        #endregion

        public async Task Parse()
        {
            System.Console.WriteLine($"Task id {taskId} start. Domain: {domain}");

            if (!WebRequestHelper.Check(baseUrl)) 
            {
                System.Console.WriteLine($"Task id {taskId} ended. Domain: {domain}");
                return;
            } 

            pageParser = new PageParser(baseUrl);
            pageParser.Page = new Page();

            await RecursiveParseInnerPages(baseUrl, 0, pageParser.Page);

            System.Console.WriteLine($"Task id {taskId} ended. Domain: {domain}");
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

            if (IsContainInnerPages(pageParser.Page))
            {
                SetExternalWebsite(pageParser.Page.InnerPages);

                Page.InnerPages = GetOnlyNewInternalPages(pageParser.Page.InnerPages, Deep);
                
                for (int i = 0, length = Page.InnerPages.Count(); i < length; i++)
                {
                    var pageUrl = GetPageUrl(Page.InnerPages.ElementAt(i).Url);

                    await RecursiveParseInnerPages(pageUrl, Deep + 1, Page.InnerPages.ElementAt(i));
                }
            }            
        }

        private bool IsContainInnerPages(Page Page)
        {
            return pageParser.Page != null && pageParser.Page.InnerPages != null;
        }

        private string GetPageUrl(string pageUrl)
        {
            if (!pageUrl.Contains(domain))
            {
                pageUrl = (pageUrl.StartsWith("/")) ? $"{baseUrl}{pageUrl}"
                                                    : $"{baseUrl}/{pageUrl}";
            }

            return pageUrl;
        }

        private List<Page> GetOnlyNewInternalPages(List<Page> Pages, int Deep)
        {
            if (Pages == null && Pages.Count == 0) return null;

            var newPages = new List<Page>();
            var pages = Pages.Where(x => x.IsExternal != true).ToList();

            for (int i = 0, length = pages.Count(); i < length; i++)
            {
                var pageUrl = pages.ElementAt(i).Url;

                if (!pageUrl.Contains(domain))
                {
                    pageUrl = (pageUrl.StartsWith("/")) ? $"{baseUrl}{pageUrl}" 
                                                        : $"{baseUrl}/{pageUrl}";
                }

                if (!DicAllInternalUrls.ContainsKey(pageUrl) && !ImageHelper.IsImage(pageUrl))
                {
                    DicAllInternalUrls.Add(pageUrl, Deep);

                    newPages.Add(pages.ElementAt(i));
                }
            }

            return newPages;
        }

        private void SetExternalWebsite(List<Page> Pages)
        {
            if (Pages == null && Pages.Count == 0) return;

            var pages = Pages.Where(x => x.IsExternal == true).ToList();

            for (int i = 0, length = pages.Count(); i < length; i++)
            {
                var pageUrl = pages.ElementAt(i).Url;
                
                var url = new Uri(pageUrl);
                var baseUrl = url.GetLeftPart(UriPartial.Authority);

                if (!WebSitesConcurrentQueue.AllWebSites.Contains(baseUrl) && Url.IsContainExtention(baseUrl, domainExtentions))
                {
                    WebSitesConcurrentQueue.WebSites.Enqueue(baseUrl);
                    WebSitesConcurrentQueue.AllWebSites.Enqueue(baseUrl);

                    Console.WriteLine($"New website added: {baseUrl}");

                    FileData.Save("websites.txt", baseUrl);
                }
            }
        }
    }
}
