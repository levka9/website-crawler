using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Logic.Modules.Interfaces;

namespace WebsiteCrawler.Logic.Services
{
    public interface IWebSiteEncodingService
    {
        Task<Encoding> GetEncodingAsync(string url, IPageDataParserModule pageDataParserModule);
    }
}
