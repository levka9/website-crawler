using System;
using System.Collections.Generic;
using System.Text;

namespace WebsiteCrawler.Model.Responses
{
    public class PageDataParserResponse
    {
        public string DomainName { get; set; }
        // webpage charset
        public Encoding Encoding { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Keywords { get; set; }
        public string Content { get; set; }
        public List<string> Emails { get; set; }
        public List<string> Phones { get; set; }
        public List<string> Links { get; set; }
        public string Address { get; set; }

    }
}
