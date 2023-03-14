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
using System.Text.RegularExpressions;
using WebsiteCrawler.Helper;
using WebsiteCrawler.Logic.Interfaces;

namespace WebsiteCrawler.Logic.Modules
{
    public class ContactPageModule : IContactPageModule
    {
        static readonly log4net.ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected string _domainName;
        IEnumerable<Page> _pages;
        HtmlDocument _htmlDocument;
        private bool _isParsed;

        public ContactPageModuleResponse ParseContactPageResponse { get; set; }
        public string DomainName
        {
            get { return _domainName; }
            set { _domainName = value; }
        }
        public bool IsParsed
        {
            get { return _isParsed; }
        }

        public async Task StartParseContactPage(string domainName, IEnumerable<Page> pages)
        {
            Init(domainName, pages);

            await GetDataFromContactPage();

            if (_htmlDocument != null)
            {
                SetEmails();
                SetPhoneNumbers();
            }
        }

        private void Init(string domainName, IEnumerable<Page> pages)
        {
            _pages = pages;
            _domainName = domainName;
            ParseContactPageResponse = new ContactPageModuleResponse();
        }

        private void SetPhoneNumbers()
        {
            ParseContactPageResponse.Phones = new List<string>();

            #region MatchByHtmlAgilityPack
            //var phoneLinks = htmlDocument.DocumentNode.SelectNodes("//a[starts-with(@href, 'tel:')]");

            //foreach (var phoneLink in phoneLinks)
            //{
            //    //var phone = phoneLink.Attributes["tel"]
            //    if (!ParseContactPageResponse.Phones.Contains(phoneLink))
            //    {
            //        ParseContactPageResponse.Phones.Add(phoneMatche.Value);
            //    }
            //} 
            #endregion

            #region MatchByRegex
            var phoneMatches = Regex.Matches(_htmlDocument.Text, RegexPatterns.PhoneNumber, RegexOptions.Singleline);

            if (phoneMatches == null || phoneMatches.Count == 0) return;

            foreach (Match phoneMatche in phoneMatches)
            {
                if (!ParseContactPageResponse.Phones.Contains(phoneMatche.Value))
                {
                    ParseContactPageResponse.Phones.Add(phoneMatche.Value);
                }
            }
            #endregion
        }

        private void SetEmails()
        {
            ParseContactPageResponse.Emails = new List<string>();

            //var emailLinks = htmlDocument.DocumentNode.SelectNodes("//a[starts-with(@href, 'mailTo:') or starts-with(@href, 'mailto:')]");
            var emailsMatched = Regex.Matches(_htmlDocument.Text, RegexPatterns.Email, RegexOptions.Singleline);

            if (emailsMatched == null && emailsMatched.Count != 0) return;

            foreach (Match emailLink in emailsMatched)
            {
                if (!ParseContactPageResponse.Emails.Contains(emailLink.Value))
                {
                    ParseContactPageResponse.Emails.Add(emailLink.Value);
                }
            }
        }

        private async Task GetDataFromContactPage()
        {
            var page = _pages?.Where(x => x.IsExternal == false &&
                                         Page.ContactPageNames.Any(y => x.Url.ToLower().Contains(y)))
                             .FirstOrDefault();

            if (page == null) return;

            page.Url = Url.GetFullUrl(_domainName, page.Url);

            Console.WriteLine($"Parse contact page {page.Url}");

            var contactPageContent = await GetHtmlPage(page.Url);

            if (!string.IsNullOrEmpty(contactPageContent))
            {
                _htmlDocument = new HtmlDocument();
                _htmlDocument.LoadHtml(contactPageContent);
                _isParsed = true;
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
                _log.Error($"PageDataParser - GetHtmlPage: {Url}", ex);
                return null;
            }
        }
    }
}