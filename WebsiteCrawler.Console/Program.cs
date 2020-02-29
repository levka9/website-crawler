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

namespace WebsiteCrawler.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
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

            multiThreadWebsiteParserRequest.DomainExtentions = new List<string>()
            {
                "co.il",
                "org.il"
            };

            multiThreadWebsiteParserRequest.MaxDeep = 0;

            var multiThreadWebsiteParser = new MultiThreadWebsiteParser(multiThreadWebsiteParserRequest);
            await multiThreadWebsiteParser.Start();

            System.Console.ReadKey();
        }
    }
}
