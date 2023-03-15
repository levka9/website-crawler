using System;
using System.Collections.Generic;
using System.Text;
using WebsiteCrawler.Model.Enums;
using Microsoft.Extensions.Logging;

namespace WebsiteCrawler.Logic
{
    public static class Url
    {
        public static string GetDomain(string baseUrl)
        {
            try
            {
                var uri = new System.Uri(baseUrl);
                return (uri != null) ? uri.Host : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static Uri GetUri(string url)
        {
            try
            {
                return new System.Uri(url);
            }
            catch
            {
                return null;
            }
        }

        public static string GetBaseUrl<T>(string domainName, ILogger<T> log)
        {
            return WebRequestHelper.IsUrlAvailable<T>($"http://{domainName}", log) ? $"http://{domainName}" : $"https://{domainName}";
        }

        public static string GetFullUrl<T>(string domainName, string pageUrl, ILogger<T> log)
        {
            var baseUrl = Url.GetBaseUrl<T>(domainName, log);

            if (!pageUrl.Contains(domainName))
            {
                pageUrl = $"{baseUrl}//{pageUrl}";
            }

            return pageUrl;
        }

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

        public static bool IsCorrectDomainLevel(string domainName, EDomainLevel EDomainLevel)
        {
            bool result = false;
            domainName = domainName.Replace("www.", "");

            switch (EDomainLevel)
            {
                case EDomainLevel.SecondLevel:
                    result = domainName.Split('.').Length == 3;
                    break;
                case EDomainLevel.ThirdLevel:
                    result = domainName.Split('.').Length == 4;
                    break;
            }

            return result;
        }

        public static bool IsContainExtention(string Url, IEnumerable<string> DomainExtentions)
        {
            foreach (var domainExtention in DomainExtentions)
            {
                if(Url.Contains(domainExtention) == true) return true;
            }

            return false;
        }
    }
}
