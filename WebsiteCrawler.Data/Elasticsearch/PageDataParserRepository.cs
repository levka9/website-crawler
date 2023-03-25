using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Data.Elasticsearch.GenericRepository;
using WebsiteCrawler.Model.Responses;

namespace WebsiteCrawler.Data.Elasticsearch
{
    public class PageDataParserRepository : ElasticsearchGenericRepository<PageDataParserModuleResponse>, 
                                            IPageDataParserRepository
    {
        public PageDataParserRepository(ElasticsearchClient client, string defaultIndexName) 
                            : base(client, defaultIndexName)
        {
            
        }
    }
}
