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
            return WebRequestHelper.IsUrlAvailable<T>($"https://{domainName}", log) ? $"https://{domainName}" : $"http://{domainName}";
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

        public static bool IsExternal(string baseUrl, string url)
        {
            bool result = false;

            baseUrl = baseUrl.Replace("www.", ".");

            try
            {
                var uri = new System.Uri(url);

                if (uri != null)
                {
                    var host = uri.Host.Replace("www.", "");

                    if (string.IsNullOrEmpty(host))
                    {
                        result = false;
                    }
                    else if (!baseUrl.Contains(host))
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

        public static bool IsJavascriptOrJumpToSection(string url)
        {
            var jumpToSectionIndex = url.IndexOf("#");
            return url.Contains("javascript:") || 
                  (jumpToSectionIndex != -1 && jumpToSectionIndex != url.Length);
        }

        public static bool IsCorrectDomainLevel(string domainName, EDomainLevel domainLevel)
        {
            bool result = false;
            domainName = domainName.Replace("www.", "");

            switch (domainLevel)
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

        public static bool IsContainExtention(string url, IEnumerable<string> domainExtentions)
        {
            foreach (var domainExtention in domainExtentions)
            {
                if(url.Contains(domainExtention) == true) return true;
            }

            return false;
        }
    }
}
