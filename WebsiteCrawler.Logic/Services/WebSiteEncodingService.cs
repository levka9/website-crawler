using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Logic.Extensions;
using WebsiteCrawler.Logic.Modules;

namespace WebsiteCrawler.Logic.Services
{
    public static class WebSiteEncodingService
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static async Task<Encoding> GetEncodingAsync(string url)
        {
            var htmlContent = await GetHtmlPageContentAsync(url);
            var pageDataParser = new PageDataParserModule();
            
            await pageDataParser.StartAsync(string.Empty, htmlContent);

            return pageDataParser.PageDataParserResponse.Encoding;
        }

        private static async Task<string> GetHtmlPageContentAsync(string url)
        {
            try
            {
                using(var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var htmlPageContent = await response.Content.ReadAsStringUtf8Async();

                    return htmlPageContent;
                }                
            }
            catch (Exception ex)
            {
                log.Error($"WebSiteEncoding - GetHtmlPage: {url}", ex);
                return null;
                //throw ex;
            }
        }
    }
}