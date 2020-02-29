using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WebsiteCrawler.Models.Requests
{
    public class WebsiteParserRequest
    {
        public string DomainName { get; set; }
        public int MaxDeep { get; set; }        
        /// <summary>
        /// Limit domain extentions
        /// </summary>
        public IEnumerable<string> DomainExtentions { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public int? TaskId { get; set; }
    }
}
