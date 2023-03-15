using System;
using System.Collections.Generic;
using System.Text;
using WebsiteCrawler.Model.Enums;

namespace WebsiteCrawler.Models.Requests
{
    public class MultiThreadWebsiteParserRequest
    {
        public IEnumerable<string> WebsiteUrls { get; set; }
        public int MaxDeep { get; set; }
        public int MaxTaskQuantity { get; set; }
        public EDomainLevel EDomainLevel { get; set; }
        /// <summary>
        /// Limit domain extentions
        /// </summary>
        public IEnumerable<string> DomainExtentions { get; set; }
    }
}
