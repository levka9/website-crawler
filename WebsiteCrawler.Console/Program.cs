﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using WebsiteCrawler.Logic;
using System.Linq;
using WebsiteCrawler.Logic.Models;

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

            string websiteUrl = "http://www.lainyan.co.il";

            var pageLinkParser = new PageParser();
            await pageLinkParser.Parse(websiteUrl, 0);
            
            await FileData.Save<Page>("links.txt", pageLinkParser.Page.InnerPages);

            System.Console.WriteLine("Hello World!");
        }
    }
}