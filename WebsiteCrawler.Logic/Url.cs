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
                var url = new System.Uri(Url);

                if (url != null)
                {
                    var host = url.Host.Replace("www.", "");

                    if (string.IsNullOrEmpty(host))
                    {
                        result = false;
                    }
                    else if (!BaseUrl.Contains(host))
                    {
                        result = true;
                    }                    
                }

                return result;
            }
            catch 
            {
                return false;
            }
        }

        public static string GetDomain(string BaseUrl)
        {            
            try
            {
                var uri = new System.Uri(BaseUrl);
                return (uri != null) ? uri.Host : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static Uri GetUri(string Url)
        {
            try
            {
                return new System.Uri(Url);
            }
            catch
            {
                return null;
            }
        }

        public static bool IsContainExtention(string Url, IEnumerable<string> DomainExtentions)
        {
            foreach (var domainExtention in DomainExtentions)
            {
                if(Url.Contains(domainExtention) == true) return true;
            }

            return false;
        }

        public static string GetBaseUrl(string DomainName)
        {
            return WebRequestHelper.Check($"http://{DomainName}") ? $"http://{DomainName}" : $"https://{DomainName}";
        }

        public static string GetFullUrl(string DomainName, string PageUrl)
        {
            var baseUrl = Url.GetBaseUrl(DomainName);

            if (!PageUrl.Contains(DomainName))
            {
                PageUrl = $"{baseUrl}//{PageUrl}";
            }

            return PageUrl;
        }
    }
}
