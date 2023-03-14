using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Models;

namespace WebsiteCrawler.Logic.Interfaces
{
    public interface IContactPageModule
    {
        Task StartParseContactPage(string domainName, IEnumerable<Page> pages);
    }
}
