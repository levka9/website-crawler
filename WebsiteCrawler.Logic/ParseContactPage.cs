using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Models;
using System.Linq;
using HtmlAgilityPack;
using WebsiteCrawler.Model.Responses;
using System.Reflection;

namespace WebsiteCrawler.Logic
{
    public class ParseContactPage
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        string domainName;
        IEnumerable<Page> pages;
        HtmlDocument htmlDocument;
        public ParseContactPageResponse ParseContactPageResponse { get; set; }

        public ParseContactPage(string DomainName, IEnumerable<Page> Pages)
        {            
            pages = Pages;
            domainName = DomainName;
            ParseContactPageResponse = new ParseContactPageResponse();
        }

        public async Task StartParseContactPage()
        {
            await GetDataFromContactPage();

            if (htmlDocument != null)
            {
                SetEmails();
                SetPhoneNumbers();
            }            
        }

        private void SetPhoneNumbers()
        {
            ParseContactPageResponse.Phones = new List<string>();

            var phoneLinks = htmlDocument.DocumentNode.SelectNodes("//a[starts-with(@href, 'tel:')]");

            if (phoneLinks == null) return;

            foreach (var link in phoneLinks)
            {
                var phone = link.Attributes["href"].Value.ToLower().Replace("tel:", "");
                
                ParseContactPageResponse.Phones.Add(phone);
            }
        }

        private void SetEmails()
        {
            ParseContactPageResponse.Emails = new List<string>();

            var emailLinks = htmlDocument.DocumentNode.SelectNodes("//a[starts-with(@href, 'mailTo:') or starts-with(@href, 'mailto:')]");

            if (emailLinks == null) return; 

            foreach (var emailLink in emailLinks)
            {
                var email = emailLink.Attributes["href"].Value.ToLower().Replace("mailto:", "");

                ParseContactPageResponse.Emails.Add(email);
            }
        }

        private async Task GetDataFromContactPage()
        {
            var page = pages.Where(x => x.IsExternal == false && x.Url.ToLower().Contains("contact")).FirstOrDefault();

            if (page == null) return;

            page.Url = Url.GetFullUrl(domainName, page.Url);

            var contactPageContent = await GetHtmlPage(page.Url);

            if (!string.IsNullOrEmpty(contactPageContent))
            {
                htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(contactPageContent);                
            }
        }

        private async Task<string> GetHtmlPage(string Url)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(Url);

                    var contentBytes = await httpClient.GetByteArrayAsync(Url);

                    string htmlPageContent = Encoding.UTF8.GetString(contentBytes);

                    return htmlPageContent;
                }
            }
            catch (Exception ex)
            {
                log.Error($"PageDataParser - GetHtmlPage: {Url}", ex);
                return null;
            }
        }
    }
}
