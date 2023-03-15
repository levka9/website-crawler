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


            var logger = new LoggerConfiguration().ReadFrom
                                                  .Configuration(configuration)
                                                  .CreateLogger();

            var serviceCollection = new ServiceCollection();
            ServiceCollections.ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var multiThreadWebsiteParser = serviceProvider.GetService<IMultiThreadWebsiteParserModule>();

            #region Log4Net Configuration
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));

            var logRepository = log4net.LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(logRepository, log4netConfig["log4net"]);
            #endregion
            
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
            multiThreadWebsiteParserRequest.MaxDeep = configuration.GetValue<int>("Parser:WebsiteParse:MaxDeep");
            multiThreadWebsiteParserRequest.MaxTaskQuantity = configuration.GetValue<int>("Parser:MultithreadParser:MaxTaskQuantity");
            multiThreadWebsiteParserRequest.EDomainLevel = Model.Enums.EDomainLevel.SecondLevel;

            await multiThreadWebsiteParser.StartAsync(multiThreadWebsiteParserRequest);

            System.Console.ReadKey();
        }
    }
}
