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
using WebsiteCrawler.Logic.Modules;

namespace WebsiteCrawler.Logic
{
    public class PageDataParser : ParseContactPage
    {
        #region Properties
        static readonly log4net.ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        HtmlDocument _htmlDocument;        
        public PageDataParserResponse PageDataParserResponse { get; set; }
        #endregion

        #region Constructors
        public PageDataParser(string domainName, string htmlContent, IEnumerable<Page> Pages = null) 
                : base(domainName, Pages)
        {
            if (!string.IsNullOrEmpty(htmlContent))
            {
                _htmlDocument = new HtmlDocument();
                _htmlDocument.LoadHtml(htmlContent);
            }
        } 
        #endregion

        public async Task StartAsync()
        {
            if (this._htmlDocument == null) return;

            PageDataParserResponse = new PageDataParserResponse();

            PageDataParserResponse.DomainName = base.domainName;
            PageDataParserResponse.Encoding = GetEncoding();
            PageDataParserResponse.Title = GetTitle();
            PageDataParserResponse.Description = GetDescription();
            PageDataParserResponse.Keywords = GetKeywords();
            //PageDataParserResponse.Links = GetAllLinks();

            await base.StartParseContactPage();
            PageDataParserResponse.Emails = base.ParseContactPageResponse.Emails;
            PageDataParserResponse.Phones = base.ParseContactPageResponse.Phones;
        }

        private Encoding? GetEncoding()
        {
            var EncodingModule = new EncodingModule(_htmlDocument.DocumentNode, base.domainName);

            return EncodingModule.GetEncoding();
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

            return (desciptionNode != null) ? desciptionNode.Attributes["content"].Value
                                            : string.Empty;
        }

        private List<string> GetKeywords()
        {
            var desciptionNode = _htmlDocument.DocumentNode.SelectSingleNode("//meta[translate(@name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='keywords']");

            return (desciptionNode != null) ? desciptionNode.Attributes["content"].Value.Split(',').ToList()
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
                _log.Error("GetAllLinks ", ex);

                return null;
            }
        }
    }
}
