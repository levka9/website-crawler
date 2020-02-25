using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using WebsiteCrawler.Models;
using System.Text;

namespace WebsiteCrawler.Logic
{
    public class PageParser : IDisposable
    {
        #region Properties
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        string url;
        string baseUrl;
        HttpClient httpClient;
        public Page Page { get; set; } 
        #endregion

        public PageParser(string BaseUrl)
        {
            baseUrl = BaseUrl;
        }

        public async Task Parse(string Url, int Deep)
        {
            url = Url;

            var htmlPageContent = await GetHtmlPage();

            GetAllLinks(htmlPageContent, Deep);

            // TODO: FilterUrls();
        }

        private async Task<string> GetHtmlPage()
        {
            try
            {
                httpClient = new HttpClient();
                //var htmlPageContent = await httpClient.GetStringAsync(this.url);

                var contentBytes = await httpClient.GetByteArrayAsync(this.url);
                
                string htmlPageContent = Encoding.UTF8.GetString(contentBytes);

                return htmlPageContent;
            }
            catch (Exception ex)
            {
                log.Error("GetHtmlPage", ex);
                return null;
                //throw ex;
            }
        }

        private Page GetAllLinks(string HtmlPageContent, int Deep)
        {
            if (string.IsNullOrEmpty(HtmlPageContent)) return null;

            Page = new Page();
            Page.HtmlPageContent = HtmlPageContent;
            Page.InnerPages = new List<Page>();

            try
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(HtmlPageContent);
                
                var links = htmlDocument.DocumentNode.SelectNodes("//a").ToArray();

                for (int i = 0, lenght = links.Count(); i < lenght; i++)
                {
                    if (links[i].Attributes["href"] != null)
                    {
                        Page.InnerPages.Add(new Page()
                        {
                            Url = links[i].Attributes["href"].Value,
                            Deep = Deep,
                            IsExternal = Url.IsExternal(baseUrl, links[i].Attributes["href"].Value)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetAllLinks ", ex);                
            }

            return Page;
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}
