using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using WebsiteCrawler.Models;
using System.Text;
using WebsiteCrawler.Logic.Extensions;
using log4net.Core;
using Microsoft.Extensions.Logging;
using WebsiteCrawler.Logic.Modules.Interfaces;

namespace WebsiteCrawler.Logic.Modules
{
    public class WebPageParserModule : IWebPageParserModule, IDisposable
    {
        #region Properties
        private string _url;
        private string _baseUrl;
        private Encoding _encoding;
        private HttpClient _httpClient;
        private ILogger<WebPageParserModule> _log;
        public Page Page { get; set; }
        #endregion

        public WebPageParserModule(ILogger<WebPageParserModule> logger)
        {
            _log = logger;
        }

        public async Task Parse(string baseUrl, string url, int deep, Encoding encoding)
        {
            _url = url;
            _baseUrl = baseUrl;
            _encoding = encoding;

            var htmlPageContent = await GetHtmlPageAsync();

            GetAllLinks(htmlPageContent, deep);

            // TODO: FilterUrls();
        }

        private async Task<string> GetHtmlPageAsync()
        {
            try
            {
                _log.LogInformation($"This url parsed:{_url}");
                
                _httpClient = new HttpClient();
                var response = await _httpClient.GetAsync(_url);
                response.EnsureSuccessStatusCode();

                var htmlPageContent = await response.Content.ReadAsStringAsync(_encoding);

                return htmlPageContent;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Url faild: {_url}");
                return null;
            }
        }

        private Page GetAllLinks(string htmlPageContent, int deep)
        {
            if (string.IsNullOrEmpty(htmlPageContent)) return null;

            Page = new Page();
            Page.HtmlPageContent = htmlPageContent;
            Page.InnerPages = new List<Page>();

            try
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlPageContent);

                var links = htmlDocument.DocumentNode.SelectNodes("//a");

                if (links == null) return null;

                foreach (var link in links)
                {
                    if (link.Attributes["href"] != null)
                    {
                        Page.InnerPages.Add(new Page()
                        {
                            Url = link.Attributes["href"].Value,
                            Deep = deep,
                            IsExternal = Url.IsExternal(_baseUrl, link.Attributes["href"].Value)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "WebPageParser - GetAllLinks ");
            }

            return Page;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
