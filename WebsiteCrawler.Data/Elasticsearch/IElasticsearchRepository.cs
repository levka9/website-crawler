using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCrawler.Data.Elasticsearch
{
    public interface IElasticsearchRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync(int page, int size);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IndexResponse> AddAsync(T entity);
        Task Update(string id, T entity);
        Task DeleteAsync(string id);
    }

}
