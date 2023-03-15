using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Model.Responses;
using WebsiteCrawler.Models;

namespace WebsiteCrawler.Logic.Modules.Interfaces
{
    public interface IPageDataParserModule
    {
        PageDataParserModuleResponse PageDataParserResponse { get; set; }
        Task StartAsync(string domainName, string htmlContent, IEnumerable<Page> pages = null);
    }
}
