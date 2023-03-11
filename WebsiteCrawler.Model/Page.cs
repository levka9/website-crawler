using System;
using System.Collections.Generic;
using System.Text;

namespace WebsiteCrawler.Models
{
    public class Page
    {        
        public string? Url { get; set; }        
        public int Deep { get; set; }
        public string? HtmlPageContent { get; set; }
        public bool IsExternal { get; set; }
        public bool IsParsed { get; set; }
        public List<Page>? InnerPages { get; set; }

        public static List<string> ContactPageNames = new List<string> 
                                                        {
                                                            "contact",
                                                            "contacts",
                                                            "contactus",
                                                            "contact-us",
                                                            "support",
                                                            "צור-קשר",
                                                            "יצירת-קשר"
                                                        };
    }
}
