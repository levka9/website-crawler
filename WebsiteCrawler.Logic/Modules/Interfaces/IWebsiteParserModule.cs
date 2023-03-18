using WebsiteCrawler.Models.Requests;

namespace WebsiteCrawler.Logic.Modules.Interfaces;

public interface IWebsiteParserModule
{
    Task ParseAsync(WebsiteParserModuleRequest websiteParserModuleRequest);
}
