using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Models.Requests;

namespace WebsiteCrawler.Logic.Modules.Interfaces
{
    public  interface IMultiThreadWebsiteParserModule
    {
        Task StartAsync(MultiThreadWebsiteParserRequest multiThreadWebsiteParserRequest);
    }
}
