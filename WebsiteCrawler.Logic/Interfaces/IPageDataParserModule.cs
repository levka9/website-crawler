using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Models;

namespace WebsiteCrawler.Logic.Interfaces
{
    public interface IPageDataParserModule
    {
        Task StartAsync(string domainName, string htmlContent, IEnumerable<Page> pages = null);
    }
}
