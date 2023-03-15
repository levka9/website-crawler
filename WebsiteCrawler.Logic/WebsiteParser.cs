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
using WebsiteCrawler.Logic.Modules;
using WebsiteCrawler.Logic.Interfaces;

namespace WebsiteCrawler.Logic
{
    public class WebsiteParser : IDisposable
    {
        #region Private params
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        int? _taskId;
        int _maxDeep;
        private int _maxInternalLinks;
        string _baseUrl;
        string _domainName;
        Encoding _encoding;
        EDomainLevel _domainLevel;
        WebPageParser _webPageParser;
        IPageDataParserModule _pageDataParserModule;
        IEnumerable<string> _domainExtentions;        
        #endregion

        #region Public params
        public int TotalPagesParsed { get; set; }
        public Dictionary<string, int> DicAllInternalUrls { get; set; }
        #endregion

        #region Constructors
        public WebsiteParser(WebsiteParserRequest WebsiteParserRequest, IPageDataParserModule pageDataParserModule)
        {
            _pageDataParserModule = pageDataParserModule;

            _taskId = WebsiteParserRequest.TaskId;
            _baseUrl = WebsiteParserRequest.DomainName;
            _maxDeep = WebsiteParserRequest.MaxDeep;
            _domainExtentions = WebsiteParserRequest.DomainExtentions;
            _domainName = WebsiteParserRequest.DomainName;
            _domainLevel = WebsiteParserRequest.DomainLevel;
            
            DicAllInternalUrls = new Dictionary<string, int>();
        } 
        #endregion

        public async Task Parse()
        {
            Console.WriteLine($"Task id {_taskId} start. Domain: {_domainName}");

            _baseUrl = Url.GetBaseUrl(_domainName);

            if (!WebRequestHelper.Check(_baseUrl)) 
            {
                Console.WriteLine($"Task id {_taskId} ended. Domain: {_domainName}");
                return;
            } 

            _encoding =  await WebSiteEncodingService.GetEncodingAsync(_baseUrl, _pageDataParserModule);

            _webPageParser = new WebPageParser(_baseUrl, _encoding);
            _webPageParser.Page = new Page();

            await RecursiveParseInnerPages(_baseUrl, 0, _webPageParser.Page);
            
            Console.WriteLine($"Task id {_taskId} ended. Domain: {_domainName}");
        }
        
        private async Task RecursiveParseInnerPages(string Url, int Deep, Page Page)
        {
            if (Deep > _maxDeep)
            {
                return;
            }

            TotalPagesParsed++;
            Console.WriteLine($"{TotalPagesParsed} - Parse: {Url}");            
            await _webPageParser.Parse(Url, Deep);

            if (IsContainInnerPages(_webPageParser.Page))
            {
                await GetHomepageContent(_webPageParser.Page.HtmlPageContent, _webPageParser.Page.InnerPages, Deep);

                SetExternalWebsite(_webPageParser.Page.InnerPages);

                Page.InnerPages = GetOnlyNewInternalPages(_webPageParser.Page.InnerPages, Deep);
                
                for (int i = 0, length = Page.InnerPages.Count(); i < length; i++)
                {
                    var pageUrl = GetPageUrl(Page.InnerPages.ElementAt(i).Url);

                    await RecursiveParseInnerPages(pageUrl, Deep + 1, Page.InnerPages.ElementAt(i));
                }
            }            
        }
                
        private async Task GetHomepageContent(string htmlPageContent, IEnumerable<Page> pages, int deep)
        {
            if (!IsHomepage(deep)) 
            {
                return;
            }

            await _pageDataParserModule.StartAsync(_domainName, htmlPageContent, pages);

            await FileData.SaveAsync("websites-content.txt", _pageDataParserModule.PageDataParserResponse.ToString(), ",");            
        }

        private bool IsContainInnerPages(Page Page)
        {
            return _webPageParser.Page != null && 
                   _webPageParser.Page.InnerPages != null && 
                   _webPageParser.Page.InnerPages.Count > 0;
        }

        private bool IsHomepage(int deep)
        {
            return deep == 0;
        }

        private string GetPageUrl(string pageUrl)
        {
            if (!pageUrl.Contains(_domainName))
            {
                pageUrl = (pageUrl.StartsWith("/")) ? $"{_baseUrl}{pageUrl}"
                                                    : $"{_baseUrl}/{pageUrl}";
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

                if (!pageUrl.Contains(_domainName))
                {
                    pageUrl = (pageUrl.StartsWith("/")) ? $"{_baseUrl}{pageUrl}" 
                                                        : $"{_baseUrl}/{pageUrl}";
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
                            Url.IsContainExtention(domainName, _domainExtentions) &&
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
            _webPageParser.Dispose();
        }
    }
}
