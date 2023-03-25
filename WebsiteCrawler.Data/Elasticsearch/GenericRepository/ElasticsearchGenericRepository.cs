using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;

namespace WebsiteCrawler.Data.Elasticsearch.GenericRepository
{
    public class ElasticsearchGenericRepository<T> : IElasticsearchGenericRepository<T> where T : class
    {
        private readonly ElasticsearchClient _client;
        private readonly string _indexName;

        public ElasticsearchGenericRepository(ElasticsearchClient client, string defaultIndexName)
        {
            _client = client;
            _indexName = defaultIndexName;
        }

        public async Task<IndexResponse> AddAsync(T entity)
        {
            var response = await _client.IndexAsync(entity, idx => idx.Index(_indexName));
            return response;
        }

        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync(int page = 1, int size = 200)
        {
            var searchResponse = await _client.SearchAsync<T>(s => s.Index(_indexName)
                                                                    .From((page - 1) * size)
                                                                    .Size(size));
            var hit = searchResponse.Hits;
            var documents = hit.Select(hit => hit.Source);
            return documents;
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var response = await _client.GetAsync<T>(id, idx => idx.Index(_indexName));
            return response.Source;
        }

        public Task Update(string id, T entity)
        {
            throw new NotImplementedException();
        }
    }
}
