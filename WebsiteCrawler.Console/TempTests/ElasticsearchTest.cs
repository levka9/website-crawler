using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Data.Elasticsearch;

namespace WebsiteCrawler.Console.TempTests
{
    public class ElasticsearchTest
    {
        private IPageDataParserRepository _pageDataParserRepository;

        public ElasticsearchTest(IPageDataParserRepository pageDataParserRepository)
        {
            _pageDataParserRepository = pageDataParserRepository;
        }

        public async Task AddAsync()
        {
            var response = await _pageDataParserRepository.AddAsync(new Model.Responses.PageDataParserModuleResponse()
            {
                Id = new Guid(),
                Address = "test",
                Content = "test",
                Description = "test",
                DomainName = "test",
                Emails = new List<string> { "test", "ss"},
                Encoding = Encoding.UTF8,
                IsContactPageParsed = true,
                Keywords = new List<string> { "test", "dd"}
            });
        }
    }
}
