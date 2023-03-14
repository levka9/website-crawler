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
using WebsiteCrawler.Logic.Interfaces;
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

            

            #region Log4Net Configuration
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));

            var logRepository = log4net.LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(logRepository, log4netConfig["log4net"]);
            #endregion

            // var encoding =  await WebSiteEncodingService.GetEncodingAsync("https://www.lainyan.co.il/");

            // using(var webPageParser = new WebPageParser("www.lainyan.co.il", encoding))
            // {
            //     await webPageParser.Parse("https://www.lainyan.co.il/", 0);
            // }
            
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

            multiThreadWebsiteParserRequest.DomainExtentions = new List<string>()
            {
                "co.il",
                "org.il"
            };

            multiThreadWebsiteParserRequest.MaxDeep = 1;

            multiThreadWebsiteParserRequest.EDomainLevel = Model.Enums.EDomainLevel.SecondLevel;

            IMultiThreadWebsiteParserModule multiThreadWebsiteParser = new MultiThreadWebsiteParserModule();
            await multiThreadWebsiteParser.StartAsync(multiThreadWebsiteParserRequest);

            System.Console.ReadKey();
        }
    }
}
