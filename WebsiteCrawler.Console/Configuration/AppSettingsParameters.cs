using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCrawler.Console.Configuration
{
    public static class AppSettingsParameters
    {
        public const string PARSER_WEBSITE_PARSE_DOMAIN_EXTENTIONS = "Parser:WebsiteParse:DomainExtentions";
        public const string PARSER_MULTITHREAD_PARSER_MAX_TASK_QUANTITY = "Parser:MultithreadParser:MaxTaskQuantity";
        public const string PARSER_WEBSITE_PARSE_MAX_DEEP = "Parser:WebsiteParse:MaxDeep";
        public const string PARSER_WEBSITE_PARSE_MAX_INTERNAL_LINKS = "Parser:WebsiteParse:MaxInternalLinks";
    }
}
