using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WebsiteCrawler.Logic
{
    public static class WebSitesConcurrentQueue
    {
        public static ConcurrentQueue<string> WebSites;
        public static ConcurrentQueue<string> AllWebSites;
    }
}
