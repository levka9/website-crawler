using Elastic.Clients.Elasticsearch;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Data.Elasticsearch.GenericRepository;
using WebsiteCrawler.Model.Responses;

namespace WebsiteCrawler.Data.Elasticsearch
{
    /// <summary>
    /// TODO: Make this repository safe in concurently way
    /// </summary>
    public class PageDataParserRepository : ElasticsearchGenericRepository<PageDataParserModuleResponse>, 
                                            IPageDataParserRepository
    {
        public PageDataParserRepository(IElasticClient client, string defaultIndexName) 
                            : base(client, defaultIndexName)
        {
            
        }
    }
}
