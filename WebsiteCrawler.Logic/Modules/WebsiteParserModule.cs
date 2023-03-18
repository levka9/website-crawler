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
using WebsiteCrawler.Logic.Modules.Interfaces;
using Microsoft.Extensions.Logging;

namespace WebsiteCrawler.Logic
{
    public class WebsiteParserModule : IWebsiteParserModule, IDisposable
    {
        #region Private params
        private int? _taskId;
        private int? _taskCounter;
        private int _maxDeep;
        private int _maxInternalLinks;
        private string _baseUrl;
        private string _domainName;
        private Encoding _encoding;
        private EDomainLevel _domainLevel;
        private IWebPageParserModule _webPageParserModule;
        private IPageDataParserModule _pageDataParserModule;
        private IEnumerable<string> _domainExtentions; 
        private ILogger<WebsiteParserModule> _log;

        private int _totalPagesParsed;
        private Dictionary<string, int> _dicAllInternalUrls;
        #endregion

        #region Public params

        #endregion

        #region Constructors
        public WebsiteParserModule(IWebPageParserModule webPageParserModule,
                                   IPageDataParserModule pageDataParserModule,
                                   ILogger<WebsiteParserModule> logger)
        {
            _webPageParserModule = webPageParserModule;
            _pageDataParserModule = pageDataParserModule;
            _log = logger;
        } 
        #endregion

        public async Task ParseAsync(WebsiteParserModuleRequest websiteParserModuleRequest)
        {
            Init(websiteParserModuleRequest);
            Console.WriteLine($"TaskId {_taskId} start. TaskCounter: {_taskCounter} Domain: {_domainName}");

            _baseUrl = Url.GetBaseUrl<WebsiteParserModule>(_domainName, _log);

            if (!WebRequestHelper.IsUrlAvailable<WebsiteParserModule>(_baseUrl, _log)) 
            {
                Console.WriteLine($"TaskId: {_taskId} ended. TaskCounter: {_taskCounter} Domain: {_baseUrl} invalid.");
                return;
            } 

            _encoding =  await WebSiteEncodingService.GetEncodingAsync(_baseUrl, _pageDataParserModule);

            await RecursiveParseInnerPages(_baseUrl, 0, new Page());
            
            Console.WriteLine($"TaskId: {_taskId} ended. TaskCounter: {_taskCounter} Domain: {_domainName}");
        }

        private void Init(WebsiteParserModuleRequest websiteParserModuleRequest)
        {
            Thread.Sleep(1000);
            
            _taskId = Task.CurrentId;
            _taskCounter = websiteParserModuleRequest.TaskCounter;
            _baseUrl = Url.GetBaseUrl<WebsiteParserModule>(websiteParserModuleRequest.DomainName, _log); ;
            _maxDeep = websiteParserModuleRequest.WebsiteParserLimitsRequest.MaxDeep;
            _maxInternalLinks = websiteParserModuleRequest.WebsiteParserLimitsRequest.MaxInternalLinks;
            _domainExtentions = websiteParserModuleRequest.DomainExtentions;
            _domainName = websiteParserModuleRequest.DomainName;
            _domainLevel = websiteParserModuleRequest.DomainLevel;
            
            _dicAllInternalUrls = new Dictionary<string, int>();

            _totalPagesParsed = 0;
        }
        
        private async Task RecursiveParseInnerPages(string url, int deep, Page page)
        {
            if (deep > _maxDeep || _totalPagesParsed > _maxInternalLinks)
            {
                return;
            }

            _totalPagesParsed++;
            Console.WriteLine($"TaskId: {_taskId} TotalPagesParsed: {_totalPagesParsed} - Parse: {url}");

            //var fullUrl = GetPageUrl(url);
            await _webPageParserModule.Parse(_baseUrl, url, deep, _encoding);

            if (IsContainInnerPages(_webPageParserModule.Page))
            {
                await GetHomepageContent(_webPageParserModule.Page.HtmlPageContent, _webPageParserModule.Page.InnerPages, deep);

                SetExternalWebsite(_webPageParserModule.Page.InnerPages);

                page.InnerPages = GetOnlyNewInternalPages(_webPageParserModule.Page.InnerPages, deep);
                
                for (int i = 0, length = page.InnerPages.Count(); i < length && _maxInternalLinks > _totalPagesParsed; i++)
                {
                    var pageUrl = GetPageUrl(page.InnerPages.ElementAt(i).Url);

                    await RecursiveParseInnerPages(pageUrl, deep + 1, page.InnerPages.ElementAt(i));
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
            return _webPageParserModule.Page != null &&
                   _webPageParserModule.Page.InnerPages != null &&
                   _webPageParserModule.Page.InnerPages.Count > 0;
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

        private List<Page> GetOnlyNewInternalPages(List<Page> pages, int deep)
        {
            if (pages == null && pages.Count == 0) return null;

            var newPages = new List<Page>();
            var internalPages = pages.Where(x => x.IsExternal != true).ToList();

            for (int i = 0, length = internalPages.Count(); i < length; i++)
            {
                var pageUrl = internalPages.ElementAt(i).Url;

                if (!pageUrl.Contains(_domainName))
                {
                    pageUrl = (pageUrl.StartsWith("/")) ? $"{_baseUrl}{pageUrl}" 
                                                        : $"{_baseUrl}/{pageUrl}";
                }

                if (!_dicAllInternalUrls.ContainsKey(pageUrl) && 
                    !ImageHelper.IsImage(pageUrl) &&
                    !Url.IsJavascriptOrJumpToSection(pageUrl))
                {
                    _dicAllInternalUrls.Add(pageUrl, deep);

                    newPages.Add(internalPages.ElementAt(i));
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
                            !Url.IsJavascriptOrJumpToSection(domainName) &&
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
            _webPageParserModule.Dispose();
        }
    }
}
