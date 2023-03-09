using HtmlAgilityPack;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Helper;

namespace WebsiteCrawler.Logic.Modules
{
    public class EncodingModule
    {
        static readonly log4net.ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private HtmlNode _documentNode;
        private string _domainName;

        public EncodingModule(HtmlNode documentNode, string domainName = "")
        {
            _documentNode = documentNode;
            _domainName = domainName;
        }

        public Encoding? GetEncoding()
        {
            var encoding = GetFromCharset();

            encoding = GetFromContentType(encoding);

            return encoding ?? Encoding.UTF8;
        }
   
        private Encoding? GetFromContentType(Encoding? encoding)
        {
            if (encoding != null) return encoding;

            var charsetNode = _documentNode.SelectSingleNode("//meta[translate(@name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='charset']");

            if (charsetNode == null)
            {
                charsetNode = _documentNode.SelectSingleNode("//meta[translate(@http-equiv,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='content-type']");

                if (charsetNode != null)
                {
                    string content = charsetNode.Attributes["content"].Value;
                    string charset = GetCharset(content);

                    encoding = GetEncodingByName(charset);
                }
            }

            return encoding;
        }

        private string GetCharset(string content)
        {
            return content.ToLower()
                          .Replace("text/html;", "")
                          .Replace("charset=", "");
        }

        private Encoding? GetFromCharset()
        {
            Encoding? encoding = null;
            var charsetNode = _documentNode.SelectSingleNode("//meta[translate(@name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='charset']");

            var charsetName = charsetNode?.InnerText;

            if(string.IsNullOrEmpty(charsetName)) 
            {
                return null;
            }

            encoding = GetEncodingByName(charsetName);

            return encoding;
        }

        private Encoding? GetEncodingByName(string charsetName)
        {
            Encoding? encoding = null;

            try
            {
                encoding = EncodingHelper.GetEncoding(charsetName.Trim());
            }
            catch (Exception ex)
            {
                _log.Warn($"Encoding of website:{_domainName} is not valid:{charsetName} exception:", ex);
            }

            return encoding;
        }
    }
}
