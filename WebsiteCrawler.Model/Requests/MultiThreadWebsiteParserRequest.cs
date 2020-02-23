using System;
using System.Collections.Generic;
using System.Text;

namespace WebsiteCrawler.Models.Requests
{
    public class MultiThreadWebsiteParserRequest
    {
        public IEnumerable<string> WebsiteUrls { get; set; }
        public int MaxDeep { get; set; }
        /// <summary>
        /// Limit domain extentions
        /// </summary>
        public IEnumerable<string> DomainExtentions { get; set; }
    }
}
