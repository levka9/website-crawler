using System;
using System.Collections.Generic;
using System.Text;

namespace WebsiteCrawler.Model.Responses
{
    public class ContactPageModuleResponse
    {
        public List<string> Emails { get; set; }
        public List<string> Phones { get; set; }
        public string Address { get; set; }

        public string ContactPageUrl { get; set; }

    }
}
