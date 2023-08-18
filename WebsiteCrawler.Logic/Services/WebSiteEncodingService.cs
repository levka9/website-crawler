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
    public class WebSiteEncodingService : IWebSiteEncodingService
    {
        private readonly ILogger _logger;

        public WebSiteEncodingService(ILogger<WebSiteEncodingService> logger)
        {
            _logger = logger;
        }

        public async Task<Encoding> GetEncodingAsync(string url, IPageDataParserModule pageDataParserModule)
        {
            var htmlContent = await GetHtmlPageContentAsync(url);
            
            await pageDataParserModule.StartAsync(string.Empty, htmlContent);

            return pageDataParserModule.PageDataParserResponse?.Encoding;
        }

        private async Task<string> GetHtmlPageContentAsync(string url)
        {
            try
            {
                using(var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(5);
                    var response = await httpClient.GetAsync(url);
                    
                    response.EnsureSuccessStatusCode();

                    var htmlPageContent = await response.Content.ReadAsStringUtf8Async();

                    return htmlPageContent;
                }                
            }
            catch (Exception ex)
            {
                _logger.LogError($"WebSiteEncoding - GetHtmlPage: {url}", ex);
                return null;
                //throw ex;
            }
        }
    }
}