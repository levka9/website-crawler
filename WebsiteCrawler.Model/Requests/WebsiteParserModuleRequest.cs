using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WebsiteCrawler.Model.Enums;

namespace WebsiteCrawler.Models.Requests
{
    public class WebsiteParserModuleRequest
    {
        public string DomainName { get; set; }
        public WebsiteParserLimitsRequest WebsiteParserLimitsRequest { get; set; }
        public EDomainLevel DomainLevel { get; set; }
        /// <summary>
        /// Limit domain extentions
        /// </summary>
        public IEnumerable<string> DomainExtentions { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public int TaskCounter { get; set; }
    }
}
