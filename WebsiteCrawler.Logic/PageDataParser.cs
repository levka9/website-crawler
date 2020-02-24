using System;
using System.Collections.Generic;
using System.Text;
using WebsiteCrawler.Model.Responses;

namespace WebsiteCrawler.Logic
{
    public class PageDataParser
    {
        string htmlContent;
        public PageDataParserResponse PageDataParserResponse { get; set; }

        public PageDataParser(string HtmlContent)
        {
            this.htmlContent = HtmlContent;
        }

        public void Start()
        {
            //PageDataParserResponse
        }
    }
}
