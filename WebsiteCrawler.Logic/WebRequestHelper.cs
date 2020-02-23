using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace WebsiteCrawler.Logic
{
    public static class WebRequestHelper
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static bool Check(string Url)
        {
            var request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "HEAD";

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                log.Error("WebRequestHelper-Check", ex);
                return false;
            }
        }
    }
}
