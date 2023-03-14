using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCrawler.Logic.Interfaces
{
    public interface IEncodingModule
    {
        Encoding? GetEncoding(HtmlNode documentNode, string domainName = "");
    }
}
