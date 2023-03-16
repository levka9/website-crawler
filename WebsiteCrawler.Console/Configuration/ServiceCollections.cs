using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Logic;
using WebsiteCrawler.Logic.Modules;
using WebsiteCrawler.Logic.Modules.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Settings.Configuration;
using System.Configuration;

namespace WebsiteCrawler.Console.Configuration
{
    public static class ServiceCollections
    {
        public static void ConfigureServices(IServiceCollection services, Serilog.Core.Logger logger) 
        {
            services.AddLogging(configure => configure.AddSerilog(logger))
                    .AddTransient<IMultiThreadWebsiteParserModule, MultiThreadWebsiteParserModule>()
                    .AddTransient<IPageDataParserModule, PageDataParserModule>()
                    .AddTransient<IContactPageModule, ContactPageModule>()
                    .AddTransient<IEncodingModule, EncodingModule>()
                    .AddTransient<IWebsiteParserModule, WebsiteParserModule>()
                    .AddTransient<IWebPageParserModule, WebPageParserModule>()
                    .BuildServiceProvider();
        }
    }
}
