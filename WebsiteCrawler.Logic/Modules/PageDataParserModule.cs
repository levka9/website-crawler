using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using WebsiteCrawler.Model.Responses;
using System.Linq;
using System.Reflection;
using WebsiteCrawler.Models;
using System.Threading.Tasks;
using System.Net.Http;
using WebsiteCrawler.Helper;
using WebsiteCrawler.Logic.Modules.Interfaces;
using Microsoft.Extensions.Logging;
using System.Configuration;

namespace WebsiteCrawler.Logic.Modules
{
    public class PageDataParserModule : IPageDataParserModule
    {
        #region Properties
        private ILogger<PageDataParserModule> _log;
        private HtmlDocument _htmlDocument;
        private IContactPageModule _contactPageModule;
        private EncodingModule _encodingModule;
        public PageDataParserModuleResponse PageDataParserResponse { get; set; }
        #endregion

        //public PageDataParserModule(IContactPageModule contactPageModule, ILogger<PageDataParserModule> logger)
        //{
        //    _log = logger;
        //    _contactPageModule = contactPageModule;
        //}

        public PageDataParserModule(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<PageDataParserModule>();
            _contactPageModule = new ContactPageModule(loggerFactory.CreateLogger<ContactPageModule>());
            _encodingModule = new EncodingModule(loggerFactory.CreateLogger<EncodingModule>());
        }

        public async Task StartAsync(string domainName, string htmlContent, IEnumerable<Page> pages = null)
        {
            Init(domainName, htmlContent, pages);

            if (_htmlDocument == null) return;

            await _contactPageModule.StartParseContactPage(domainName, pages);

            PrepareResponse();
        }

        private void PrepareResponse()
        {
            PageDataParserResponse = new PageDataParserModuleResponse();

            PageDataParserResponse.DomainName = _contactPageModule.DomainName;
            PageDataParserResponse.Encoding = GetEncoding();
            PageDataParserResponse.Title = GetTitle();
            PageDataParserResponse.Description = GetDescription();
            PageDataParserResponse.Keywords = GetKeywords();
            //PageDataParserResponse.Links = GetAllLinks();


            PageDataParserResponse.Emails = _contactPageModule.ParseContactPageResponse.Emails;
            PageDataParserResponse.Phones = _contactPageModule.ParseContactPageResponse.Phones;
            PageDataParserResponse.ContactPageUrl = _contactPageModule.ParseContactPageResponse.ContactPageUrl;
            PageDataParserResponse.IsContactPageParsed = _contactPageModule.IsParsed;
        }

        private void Init(string domainName, string htmlContent, IEnumerable<Page> pages = null)
        {
            if (!string.IsNullOrEmpty(htmlContent))
            {
                _htmlDocument = new HtmlDocument();
                _htmlDocument.LoadHtml(htmlContent);
            }
        }

        private Encoding? GetEncoding()
        {
            return _encodingModule.GetEncoding(_htmlDocument.DocumentNode, _contactPageModule.DomainName);
        }

        private string GetTitle()
        {
            var titleNode = _htmlDocument.DocumentNode.SelectSingleNode("//title");

            if (titleNode != null)
            {
                return titleNode.InnerText;
            }

            return string.Empty;
        }

        private string GetDescription()
        {
            var desciptionNode = _htmlDocument.DocumentNode.SelectSingleNode("//meta[translate(@name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='description']");

            return desciptionNode != null ? desciptionNode.Attributes["content"].Value
                                            : string.Empty;
        }

        private List<string> GetKeywords()
        {
            var desciptionNode = _htmlDocument.DocumentNode.SelectSingleNode("//meta[translate(@name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='keywords']");

            return desciptionNode != null ? desciptionNode.Attributes["content"]
                                                          .Value.Split(',')
                                                          .Where(x => !string.IsNullOrEmpty(x))
                                                          .ToList()
                                          : null;
        }

        private List<string> GetAllLinks()
        {
            try
            {
                var links = new List<string>();
                var linksNode = _htmlDocument.DocumentNode.SelectNodes("//a").ToArray();

                if (linksNode == null) return null;

                for (int i = 0, lenght = linksNode.Count(); i < lenght; i++)
                {
                    if (linksNode[i].Attributes["href"] != null)
                    {
                        links.Add(linksNode[i].Attributes["href"].Value);
                    }
                }

                return links;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "GetAllLinks ");

                return null;
            }
        }
    }
}
