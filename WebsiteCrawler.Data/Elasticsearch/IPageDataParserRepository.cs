using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Data.Elasticsearch.GenericRepository;
using WebsiteCrawler.Model.Responses;

namespace WebsiteCrawler.Data.Elasticsearch
{
    public interface IPageDataParserRepository : IElasticsearchGenericRepository<PageDataParserModuleResponse> 
    {
    }
}
