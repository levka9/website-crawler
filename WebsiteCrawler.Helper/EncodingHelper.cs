using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCrawler.Helper
{
    // This encoding helper works with provider 
    // that support unsupported encoding types like windows-1255  
    public static class EncodingHelper
    {

        public static Encoding GetEncoding(string encodingName)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(encodingName);
        }

        public static Encoding GetEncoding(int encodingCode)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(encodingCode);
        }
    }
}