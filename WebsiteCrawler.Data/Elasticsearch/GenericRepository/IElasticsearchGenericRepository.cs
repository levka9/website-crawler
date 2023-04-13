﻿using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCrawler.Data.Elasticsearch.GenericRepository
{
    public interface IElasticsearchGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync(int page, int size);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IndexResponse> AddAsync(T entity);
        Task Update(string id, T entity);
        Task DeleteAsync(string id);
    }

}
