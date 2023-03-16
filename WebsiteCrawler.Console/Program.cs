using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using WebsiteCrawler.Logic;
using System.Linq;
using WebsiteCrawler.Models;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using WebsiteCrawler.Models.Requests;
using System.Linq;
using WebsiteCrawler.Logic.Services;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Settings.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebsiteCrawler.Logic.Modules.Interfaces;
using WebsiteCrawler.Console.Configuration;
using WebsiteCrawler.Logic.Modules;
using Serilog.Formatting.Compact;
using System.Text;
using Serilog.Events;

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
            ServiceCollections.ConfigureServices(serviceCollection, logger);
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var multiThreadWebsiteParser = serviceProvider.GetService<IMultiThreadWebsiteParserModule>();

            /* Website Parser */
            //await OneThreadWebsiteParser.Start();

            var multiThreadWebsiteParserRequest = new MultiThreadWebsiteParserRequest();
            multiThreadWebsiteParserRequest.WebsiteUrls = new List<string>()
            {
                "www.2net.co.il",
                "www.lainyan.co.il",
                "www.a.co.il",
                "www.startpage.co.il"
            };

            multiThreadWebsiteParserRequest.DomainExtentions = configuration.GetSection("Parser:WebsiteParse:DomainExtentions").Get<string[]>();
            multiThreadWebsiteParserRequest.MaxTaskQuantity = configuration.GetValue<int>("Parser:MultithreadParser:MaxTaskQuantity");
            multiThreadWebsiteParserRequest.WebsiteParserLimits = new WebsiteParserLimitsRequest();
            multiThreadWebsiteParserRequest.WebsiteParserLimits.MaxDeep = configuration.GetValue<int>("Parser:WebsiteParse:MaxDeep");
            multiThreadWebsiteParserRequest.WebsiteParserLimits.MaxInternalLinks = configuration.GetValue<int>("Parser:WebsiteParse:MaxInternalLinks");
            multiThreadWebsiteParserRequest.EDomainLevel = Model.Enums.EDomainLevel.SecondLevel;

            await multiThreadWebsiteParser.StartAsync(multiThreadWebsiteParserRequest);

            System.Console.ReadKey();
        }
    }
}
