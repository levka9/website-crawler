using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using WebsiteCrawler.Logic.Models;

namespace WebsiteCrawler.Logic
{
    public class PageParser
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        string url;
        HttpClient httpClient;
        public Page Page { get; set; }               

        public async Task Parse(string Url, int Deep)
        {
            url = Url;

            var htmlPage = await GetHtmlPage();

            GetAllLinks(htmlPage, Deep);

            // TODO: FilterUrls();
        }

        private async Task<string> GetHtmlPage()
        {
            try
            {
                httpClient = new HttpClient();
                var htmlPage = await httpClient.GetStringAsync(this.url);

                return htmlPage;
            }
            catch (Exception ex)
            {
                log.Error("GetHtmlPage", ex);
                throw ex;
            }
        }

        private Page GetAllLinks(string HtmlPage, int Deep)
        {
            Page = new Page();
            Page.InnerPages = new List<Page>();

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(HtmlPage);

            var links = htmlDocument.DocumentNode.SelectNodes("//a").ToArray();

            for (int i = 0, lenght = links.Count(); i < lenght; i++)
            {
                if (links[i].Attributes["href"] != null)
                {
                    Page.InnerPages.Add(new Page()
                    {
                        Url = links[i].Attributes["href"].Value,
                        Deep = Deep,
                        IsExternal = Url.IsExternal(url, links[i].Attributes["href"].Value)                        
                    });
                }                
            }

            return Page;
        }
    }
}
