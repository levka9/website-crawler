using System;

namespace WebsiteCrawler.Helper
{
    public static class RegexPatterns
    {
        public static readonly string PhoneNumber = @"(0\d+-\d+-\d+)|(0\d+-\d+)";
        public static readonly string Email = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
    }
}
