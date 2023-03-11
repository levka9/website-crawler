using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Models;
using System.Linq;
using WebsiteCrawler.Models.Requests;
using System.Threading;
using WebsiteCrawler.Model.Responses;
using System.Collections.Concurrent;
using System.Reflection;
using WebsiteCrawler.Model.Enums;
using WebsiteCrawler.Logic.Services;

namespace WebsiteCrawler.Logic
{
    public class WebsiteParser : IDisposable
    {
        #region Private params
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        int? taskId;
        int maxDeep;        
        string baseUrl;
        string domainName;
        Encoding encoding;
        EDomainLevel domainLevel;
        WebPageParser webPageParser;
        PageDataParser pageDataParser;
        IEnumerable<string> domainExtentions;        
        #endregion

        #region Public params
        public int TotalPagesParsed { get; set; }
        public Dictionary<string, int> DicAllInternalUrls { get; set; }
        #endregion

        #region Constructors
        public WebsiteParser(WebsiteParserRequest WebsiteParserRequest)
        {
            taskId = WebsiteParserRequest.TaskId;
            baseUrl = WebsiteParserRequest.DomainName;
            maxDeep = WebsiteParserRequest.MaxDeep;
            domainExtentions = WebsiteParserRequest.DomainExtentions;
            domainName = WebsiteParserRequest.DomainName;
            domainLevel = WebsiteParserRequest.DomainLevel;
            
            DicAllInternalUrls = new Dictionary<string, int>();
        } 
        #endregion

        public async Task Parse()
        {
            Console.WriteLine($"Task id {taskId} start. Domain: {domainName}");

            baseUrl = Url.GetBaseUrl(domainName);

            if (!WebRequestHelper.Check(baseUrl)) 
            {
                Console.WriteLine($"Task id {taskId} ended. Domain: {domainName}");
                return;
            } 

            encoding =  await WebSiteEncodingService.GetEncodingAsync(baseUrl);

            webPageParser = new WebPageParser(baseUrl, encoding);
            webPageParser.Page = new Page();

            await RecursiveParseInnerPages(baseUrl, 0, webPageParser.Page);

            //try
            //{
            //    //await RecursiveParseInnerPages(baseUrl, 0, WebPageParser.Page);
            //}
            //catch (LockRecursionException ex)
            //{
            //    log.Error("WebsiteParser - RecursiveParseInnerPages", ex);
            //    throw ex;
            //}

            Console.WriteLine($"Task id {taskId} ended. Domain: {domainName}");
        }
        
        private async Task RecursiveParseInnerPages(string Url, int Deep, Page Page)
        {
            if (Deep > maxDeep)
            {
                return;
            }

            TotalPagesParsed++;
            Console.WriteLine($"{TotalPagesParsed} - Parse: {Url}");            
            await webPageParser.Parse(Url, Deep);

            if (IsContainInnerPages(webPageParser.Page))
            {
                await GetHomepageContent(webPageParser.Page.HtmlPageContent, webPageParser.Page.InnerPages, Deep);

                SetExternalWebsite(webPageParser.Page.InnerPages);

                Page.InnerPages = GetOnlyNewInternalPages(webPageParser.Page.InnerPages, Deep);
                
                for (int i = 0, length = Page.InnerPages.Count(); i < length; i++)
                {
                    var pageUrl = GetPageUrl(Page.InnerPages.ElementAt(i).Url);

                    await RecursiveParseInnerPages(pageUrl, Deep + 1, Page.InnerPages.ElementAt(i));
                }
            }            
        }
                
        private async Task GetHomepageContent(string HtmlPageContent, IEnumerable<Page> Pages, int deep)
        {
            if (deep == 0)
            {
                pageDataParser = new PageDataParser(domainName, HtmlPageContent, Pages);
                await pageDataParser.StartAsync();

                await FileData.SaveAsync<PageDataParserResponse>("websites-content.txt", pageDataParser.PageDataParserResponse, ",");
            }            
        }

        private bool IsContainInnerPages(Page Page)
        {
            return webPageParser.Page != null && 
                   webPageParser.Page.InnerPages != null && 
                   webPageParser.Page.InnerPages.Count > 0;
        }

        private string GetPageUrl(string pageUrl)
        {
            if (!pageUrl.Contains(domainName))
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

                if (!pageUrl.Contains(domainName))
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
                
                var url = Url.GetUri(pageUrl);

                if (url != null)
                {
                    var baseUrl = url.GetLeftPart(UriPartial.Authority);

                    if (!string.IsNullOrEmpty(baseUrl))
                    {
                        var domainName = Url.GetDomain(baseUrl);

                        if (!WebSitesConcurrentQueue.AllWebSites.Contains(domainName) && 
                            Url.IsContainExtention(domainName, domainExtentions) &&
                            Url.IsCorrectDomainLevel(domainName, Model.Enums.EDomainLevel.SecondLevel))
                        {
                            WebSitesConcurrentQueue.WebSites.Enqueue(domainName);
                            WebSitesConcurrentQueue.AllWebSites.Enqueue(domainName);

                            Console.WriteLine($"New website added: {domainName}");

                            FileData.Save("websites.txt", domainName);
                        }
                    }
                }    
            }
        }

        public void Dispose()
        {
            webPageParser.Dispose();
        }
    }
}
