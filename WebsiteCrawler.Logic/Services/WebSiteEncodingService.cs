using log4net.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Logic.Extensions;
using WebsiteCrawler.Logic.Modules.Interfaces;
using WebsiteCrawler.Logic.Modules;

namespace WebsiteCrawler.Logic.Services
{
    public static class WebSiteEncodingService
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static async Task<Encoding> GetEncodingAsync(string url, IPageDataParserModule pageDataParserModule)
        {
            var htmlContent = await GetHtmlPageContentAsync(url);
            
            await pageDataParserModule.StartAsync(string.Empty, htmlContent);

            return pageDataParserModule.PageDataParserResponse.Encoding;
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