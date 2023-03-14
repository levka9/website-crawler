using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace WebsiteCrawler.Logic
{
    public static class ImageHelper
    {
        private static readonly List<string> imagesExtensions = new List<string>() { ".jpg", ".jpeg", ".bmp", ".gif", ".png"};

        public static bool IsImage(string urlPath)
        {
            var indexOfDot = urlPath.LastIndexOf(".");
            var imageExtensionName = urlPath.Substring(indexOfDot, urlPath.Length - indexOfDot).ToLower();

            var imageExtension = imagesExtensions.Where(x => x.Contains(imageExtensionName)).FirstOrDefault();

            return (imageExtension == null) ? false : true;
        }
    }
}
