using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Models;

namespace WebsiteCrawler.Logic.Modules.Interfaces
{
    public interface IWebPageParserModule
    {
        Page Page { get; set; }
        Task Parse(string baseUrl, string url, int deep, Encoding encoding);
        void Dispose();
    }
}
