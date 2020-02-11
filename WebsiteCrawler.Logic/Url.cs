using System;
using System.Collections.Generic;
using System.Text;

namespace WebsiteCrawler.Logic
{
    public static class Url
    {
        public static bool IsExternal(string BaseUrl, string Url)
        {
            bool result = false;

            BaseUrl = BaseUrl.Replace("www.", ".");

            try
            {
                var host = new System.Uri(Url).Host;

                if (string.IsNullOrEmpty(host))
                {
                    result = false;
                }
                else if (!BaseUrl.Contains(host))
                {
                    result = true;
                }

                return result;
            }
            catch 
            {
                return false;
            }         
        }
    }
}
