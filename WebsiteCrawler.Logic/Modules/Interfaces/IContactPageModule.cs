using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Models;
using WebsiteCrawler.Model.Responses;

namespace WebsiteCrawler.Logic.Modules.Interfaces
{
    public interface IContactPageModule
    {
        string DomainName { get; set; }
        bool IsParsed { get; }
        ContactPageModuleResponse ParseContactPageResponse { get; set; }
        Task StartParseContactPage(string domainName, IEnumerable<Page> pages);
    }
}
