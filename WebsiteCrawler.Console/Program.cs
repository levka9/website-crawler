using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Settings.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using WebsiteCrawler.Console.Configuration;
using WebsiteCrawler.Console.TempTests;
using WebsiteCrawler.Data.Elasticsearch;
using WebsiteCrawler.Logic;
using WebsiteCrawler.Logic.Modules;
using WebsiteCrawler.Logic.Modules.Interfaces;
using WebsiteCrawler.Logic.Services;
using WebsiteCrawler.Models;
using WebsiteCrawler.Models.Requests;

namespace WebsiteCrawler.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                          .AddJsonFile("appsettings.json")
                                                          .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
                                                          .Build();

            var logger = SerilogConfig.Get(configuration);

            var serviceCollection = new ServiceCollection();
            ServiceCollections.ConfigureServices(serviceCollection, configuration, logger);
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            //var multiThreadWebsiteParser = serviceProvider.GetService<IMultiThreadWebsiteParserModule>();
            var loggerFactory = new LoggerFactory();
            var pageDataParserRepository = serviceProvider.GetService<IPageDataParserRepository>();
            var multiThreadWebsiteParser = new MultiThreadWebsiteParserModule(loggerFactory, pageDataParserRepository);

            //var pageDataParserRepository = serviceProvider.GetService<IPageDataParserRepository>(); 
            //var elasticsearchTest = new ElasticsearchTest(pageDataParserRepository);
            //await elasticsearchTest.AddAsync();

            /* Website Parser */
            //await OneThreadWebsiteParser.Start(serviceProvider);

            //TestMultithreadTasks testMultithreadTasks = new TestMultithreadTasks();
            //await testMultithreadTasks.Start();

            //System.Console.ReadKey();

            var multiThreadWebsiteParserRequest = PrepareRequest(configuration);

            await multiThreadWebsiteParser.StartAsync(multiThreadWebsiteParserRequest);

            System.Console.ReadKey();
        }

        public static MultiThreadWebsiteParserRequest PrepareRequest(IConfigurationRoot? configuration)
        {
            var multiThreadWebsiteParserRequest = new MultiThreadWebsiteParserRequest();
            multiThreadWebsiteParserRequest.WebsiteUrls = new List<string>()
            {
                "www.2net.co.il",
                "www.lainyan.co.il",
                "www.a.co.il",
                "www.startpage.co.il"
            };

            multiThreadWebsiteParserRequest.DomainExtentions =
                configuration.GetSection(AppSettingsParameters.PARSER_WEBSITE_PARSE_DOMAIN_EXTENTIONS).Get<string[]>();
            multiThreadWebsiteParserRequest.MaxTaskQuantity =
                configuration.GetValue<int>(AppSettingsParameters.PARSER_MULTITHREAD_PARSER_MAX_TASK_QUANTITY);
            multiThreadWebsiteParserRequest.WebsiteParserLimits = new WebsiteParserLimitsRequest();
            multiThreadWebsiteParserRequest.WebsiteParserLimits.MaxDeep =
                configuration.GetValue<int>(AppSettingsParameters.PARSER_WEBSITE_PARSE_MAX_DEEP);
            multiThreadWebsiteParserRequest.WebsiteParserLimits.MaxInternalLinks =
                configuration.GetValue<int>(AppSettingsParameters.PARSER_WEBSITE_PARSE_MAX_INTERNAL_LINKS);
            multiThreadWebsiteParserRequest.EDomainLevel = Model.Enums.EDomainLevel.SecondLevel;

            return multiThreadWebsiteParserRequest;
        }
    }
}
