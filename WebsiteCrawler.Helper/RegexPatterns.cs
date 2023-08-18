using System;

namespace WebsiteCrawler.Helper
{
    public static class RegexPatterns
    {
        public static readonly string PhoneNumber = @"(\+972[0-9-]+)|(0[0-9-]*)";
        // public static readonly string PhoneNumber = @"(\+\d{1,2}\s?)?1?\-?\.?\s?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}";
        
        // public static readonly string Email = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
        public static readonly string Email = @"([\S]+@[\S]+)";
    }
}
