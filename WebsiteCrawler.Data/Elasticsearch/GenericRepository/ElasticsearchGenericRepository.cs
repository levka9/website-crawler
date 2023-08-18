using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Nest;
using WebsiteCrawler.Data.Models;
using WebsiteCrawler.Model.Base;
using WebsiteCrawler.Model.Responses;

namespace WebsiteCrawler.Data.Elasticsearch.GenericRepository
{
    public class ElasticsearchGenericRepository<T> : IElasticsearchGenericRepository<T> where T : BaseResponse
    {
        private readonly IElasticClient _client;
        private readonly string _indexName;

        public ElasticsearchGenericRepository(IElasticClient client, string defaultIndexName)
        {
            _client = client;
            _indexName = defaultIndexName;
        }

        public async Task<IndexResponse> AddAsync(T entity)
        {
            var response = await _client.IndexAsync(entity, idx => idx.Index(_indexName)
                                                                      .Id(entity.Id));
            return response;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var response = await _client.DeleteAsync<T>(id, idx => idx.Index(_indexName));

            if (!response.IsValid)
            {
                throw new Exception(response.OriginalException.Message);
            }
            
            return response.IsValid;
        }

        public async Task<IEnumerable<T>> FindAsync(ElasticsearchRequest request)
        {
            var dynamicQuery = new List<QueryContainer>();
            foreach (var item in request.Fields)
            {
                dynamicQuery.Add(Query<T>.Match(m => m.Field(new Field(item.Key.ToLower())).Query(item.Value)));
            }

            var result = await _client.SearchAsync<T>(s => s
                                           .From(request.From)
                                           .Size(request.Size)
                                           .Index(_indexName)
                                           .Query(q => q.Bool(b => b.Must(dynamicQuery.ToArray()))));

            if (!result.IsValid)
            {
                throw new Exception(result.OriginalException.Message);
            }

            return result.Documents;
        }

        public IEnumerable<T> GetAll(int page = 0, int size = 200)
        {
            ValidateAndSetSize(ref size);

            return _client.Search<T>(s => s.From(page)
                                           .Size(size)
                                           .MatchAll()
                                           .Index(_indexName)).Documents;
        }

        public async Task<IEnumerable<T>> GetAllAsync(int page = 0, int size = 200)
        {
            ValidateAndSetSize(ref size);

            return (await _client.SearchAsync<T>(s => s.From(page)
                                                       .Size(size)
                                                       .MatchAll()
                                                       .Index(_indexName)
                                                       )).Documents;
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

        private void ValidateAndSetSize(ref int size)
        {
            var maxEntitiesResponse = _client.Count<T>();
            var maxEntitiesCount = (int)maxEntitiesResponse.Count;

            size = (size > maxEntitiesCount) ? maxEntitiesCount : size;
        }
    }
}
