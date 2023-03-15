using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;

namespace WebsiteCrawler.Logic
{
    public static class WebRequestHelper
    {
        public static bool IsUrlAvailable<T>(string url, ILogger<T> log)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "HEAD";

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"WebRequestHelper - Check: {url}");
                return false;
            }
        }
    }
}
