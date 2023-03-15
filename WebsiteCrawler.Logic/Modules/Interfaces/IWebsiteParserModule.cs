using WebsiteCrawler.Models.Requests;

namespace WebsiteCrawler.Logic.Modules.Interfaces;

public interface IWebsiteParserModule
{
    int TotalPagesParsed { get; set; }
    Dictionary<string, int> DicAllInternalUrls { get; set; }
    Task Parse(WebsiteParserModuleRequest websiteParserModuleRequest);
}
