using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Logic.Interfaces;
using WebsiteCrawler.Logic;
using WebsiteCrawler.Logic.Modules;

namespace WebsiteCrawler.Console.Configuration
{
    public static class ServiceCollections
    {
        public static void Add() 
        {
            var serviceProvider = new ServiceCollection()
                                        .AddTransient<IMultiThreadWebsiteParserModule, MultiThreadWebsiteParserModule>()
                                        .AddTransient<IPageDataParserModule, PageDataParserModule>()
                                        .AddTransient<IContactPageModule, ContactPageModule>()
                                        .AddTransient<IEncodingModule, EncodingModule>()
                                        .BuildServiceProvider();
        }
    }
}
