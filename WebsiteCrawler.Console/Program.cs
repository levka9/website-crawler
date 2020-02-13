using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using WebsiteCrawler.Logic;
using System.Linq;
using WebsiteCrawler.Logic.Models;
using System.Collections.Generic;
using System.Threading;

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

            var websites = new Queue<string>();
            websites.Enqueue("https://www.nytimes.com");
            websites.Enqueue("https://buywordpress.co.il");
            websites.Enqueue("https://www.mgweb.co.il");
            websites.Enqueue("https://habr.com");
            websites.Enqueue("https://skyeng.ru");
            websites.Enqueue("https://docs.microsoft.com");


            var multiThreadWebsiteParser = new MultiThreadWebsiteParser(websites, 1);
            multiThreadWebsiteParser.Start();

            System.Console.ReadKey();
        }
    }
}
