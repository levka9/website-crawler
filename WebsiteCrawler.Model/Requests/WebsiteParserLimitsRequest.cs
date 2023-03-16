using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WebsiteCrawler.Model.Enums;

namespace WebsiteCrawler.Models.Requests
{
    public class WebsiteParserLimitsRequest
    {
        public int MaxDeep { get; set; }
        public int MaxInternalLinks { get; set; }
    }
}
