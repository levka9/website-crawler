using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using WebsiteCrawler.Logic;
using System.Linq;
using WebsiteCrawler.Logic.Models;
using System.Collections.Generic;

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
            //var websiteParser = new WebsiteParser("https://www.mgweb.co.il");
            //var websiteParser = new WebsiteParser("https://buywordpress.co.il");
            //var websiteParser = new WebsiteParser("https://proxy6.net");

            var websiteParser = new WebsiteParser("https://www.nytimes.com", 2);
            await websiteParser.Parse();

            await FileData.Save<object>("links.txt", websiteParser.DicAllInternalUrls.Select(x=> new { url = x.Key, deep = x.Value }));

            System.Console.ReadKey();
        }
    }
}
