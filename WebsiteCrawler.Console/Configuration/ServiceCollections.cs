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
using WebsiteCrawler.Model.Responses;
using WebsiteCrawler.Data.Elasticsearch.GenericRepository;
using WebsiteCrawler.Data.Elasticsearch;

namespace WebsiteCrawler.Console.Configuration
{
    public static class ServiceCollections
    {
        public static void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration, Serilog.Core.Logger logger) 
        {
            var defuaultIndex = configuration.GetValue<string>("Elasticsearch:DefaultIndex");
            var elasticsearchClient = ElasticsearchClientConfig.GetClient(configuration);

            services.AddLogging(configure => configure.AddSerilog(logger))
                    .AddTransient<IMultiThreadWebsiteParserModule, MultiThreadWebsiteParserModule>()
                    .AddTransient<IPageDataParserModule, PageDataParserModule>()
                    .AddTransient<IContactPageModule, ContactPageModule>()
                    .AddTransient<IEncodingModule, EncodingModule>()
                    .AddTransient<IWebsiteParserModule, WebsiteParserModule>()
                    .AddTransient<IWebPageParserModule, WebPageParserModule>()
                    .AddTransient<IPageDataParserRepository>(x => new PageDataParserRepository(elasticsearchClient, defuaultIndex))
                    .AddSingleton(s => ElasticsearchClientConfig.GetClient(configuration));
                    //.BuildServiceProvider();
        }
    }
}
