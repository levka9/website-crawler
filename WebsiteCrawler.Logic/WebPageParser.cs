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

namespace WebsiteCrawler.Logic
{
    public class WebPageParser : IDisposable
    {
        #region Properties
        static readonly log4net.ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        string _url;
        string _baseUrl;
        Encoding _encoding;
        HttpClient _httpClient;
        public Page Page { get; set; } 
        #endregion

        public WebPageParser(string baseUrl, Encoding encoding)
        {
            _baseUrl = baseUrl;
        }

        public async Task Parse(string url, int deep)
        {
            _url = url;

            var htmlPageContent = await GetHtmlPageAsync();

            GetAllLinks(htmlPageContent, deep);

            // TODO: FilterUrls();
        }

        private async Task<string> GetHtmlPageAsync()
        {
            try
            {
                _httpClient = new HttpClient();
                var response = await _httpClient.GetAsync(_url);
                response.EnsureSuccessStatusCode();

                var htmlPageContent = await response.Content.ReadAsStringAsync(_encoding);
                
                return htmlPageContent;
            }
            catch (Exception ex)
            {
                _log.Error($"WebPageParser - GetHtmlPage: {_url}", ex);
                return null;
                //throw ex;
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
                _log.Error("WebPageParser - GetAllLinks ", ex);                
            }

            return Page;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
