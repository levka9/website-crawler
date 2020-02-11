using System;
using System.Collections.Generic;
using System.Text;

namespace WebsiteCrawler.Logic.Models
{
    public class Page
    {        
        public string Url { get; set; }
        public int Deep { get; set; }
        public bool IsExternal { get; set; }
        public bool IsParsed { get; set; }
        public List<Page> InnerPages { get; set; }
    }
}
