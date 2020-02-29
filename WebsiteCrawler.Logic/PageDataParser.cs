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

namespace WebsiteCrawler.Logic
{
    public class PageDataParser : ParseContactPage
    {
        #region Properties
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        HtmlDocument htmlDocument;        
        public PageDataParserResponse PageDataParserResponse { get; set; }
        #endregion

        #region Constructors
        public PageDataParser(string domainName, string HtmlContent, IEnumerable<Page> Pages) : base(domainName, Pages)
        {
            if (!string.IsNullOrEmpty(HtmlContent))
            {
                htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(HtmlContent);
            }
        } 
        #endregion

        public async Task Start(string domainName)
        {
            if (this.htmlDocument == null) return;

            PageDataParserResponse = new PageDataParserResponse();

            PageDataParserResponse.DomainName = domainName;
            PageDataParserResponse.Title = GetTitle();
            PageDataParserResponse.Description = GetDescription();
            PageDataParserResponse.Keywords = GetKeywords();
            //PageDataParserResponse.Links = GetAllLinks();

            await base.StartParseContactPage();
            PageDataParserResponse.Emails = base.ParseContactPageResponse.Emails;
            PageDataParserResponse.Phones = base.ParseContactPageResponse.Phones;
        }

        private string GetTitle()
        {
            var titleNode = htmlDocument.DocumentNode.SelectSingleNode("//title");

            if (titleNode != null)
            {
                return titleNode.InnerText;
            }

            return string.Empty;
        }

        private string GetDescription()
        {
            var desciptionNode = htmlDocument.DocumentNode.SelectSingleNode("//meta[translate(@name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='description']");

            return (desciptionNode != null) ? desciptionNode.Attributes["content"].Value
                                            : string.Empty;
        }

        private List<string> GetKeywords()
        {
            var desciptionNode = htmlDocument.DocumentNode.SelectSingleNode("//meta[translate(@name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='keywords']");

            return (desciptionNode != null) ? desciptionNode.Attributes["content"].Value.Split(',').ToList()
                                            : null;
        }

        private List<string> GetAllLinks()
        {
            try
            {
                var links = new List<string>();
                var linksNode = htmlDocument.DocumentNode.SelectNodes("//a").ToArray();

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
                log.Error("GetAllLinks ", ex);

                return null;
            }
        }

        
    }
}
