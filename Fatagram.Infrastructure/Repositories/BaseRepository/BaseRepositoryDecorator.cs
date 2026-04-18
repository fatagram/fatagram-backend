using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Cache;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;

namespace Fatagram.Infrastructure.Repositories.BaseRepository
{
    public abstract class BaseRepositoryDecorator<T>(
        IBaseRepository<T> inner,
        AppDbContext dbContext
    ) : IBaseRepository<T>
        where T : BaseEntity
    {
        protected readonly IBaseRepository<T> _inner = inner;
        protected readonly AppDbContext _dbContext = dbContext;

        public virtual async Task<T> AddAsync(T entity) => await _inner.AddAsync(entity);

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null) =>
            await _inner.CountAsync(filter);

        public virtual async Task DeleteAsync(Guid id) => await _inner.DeleteAsync(id);

        public virtual async Task DeleteAsync(T entity) => await _inner.DeleteAsync(entity);

        public async Task<TResult> ExecuteTransactionAction<TResult>(Func<Task<TResult>> action) =>
            await _inner.ExecuteTransactionAction(action);

        public async Task ExecuteTransactionAction(Func<Task> action) =>
            await _inner.ExecuteTransactionAction(action);

        public virtual async Task<List<TResult>> GetAllAsync<TResult, TKey>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, TKey>>? orderBy = null,
            bool orderDesc = false,
            int? limit = null,
            TKey? lastKey = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null
        )
            where TKey : struct, IComparable<TKey> =>
            await _inner.GetAllAsync(selector, filter, orderBy, orderDesc, limit, lastKey, include);

        public virtual async Task<List<TResult>> GetAllAsync<TResult, TKey>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, TKey>>? orderBy = null,
            bool orderDesc = false,
            int? limit = null,
            TKey? lastKey = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null
        )
            where TKey : struct, IComparable<TKey> =>
            await _inner.GetAllAsync<TResult, TKey>(
                filter,
                orderBy,
                orderDesc,
                limit,
                lastKey,
                include
            );

        public virtual async Task<List<TResult>> GetAllAsync<TResult>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, TResult>>? selector = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null
        ) => await _inner.GetAllAsync(filter, selector, include);

        public virtual async Task<TResult?> GetAsync<TResult>(
            Guid id,
            Expression<Func<T, TResult>>? selector = null
        ) => await _inner.GetAsync(id, selector);

        public virtual async Task<TResult?> GetByUniqueAsync<TResult>(
            Expression<Func<T, bool>> uniquePredicate,
            Expression<Func<T, TResult>>? selector = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null
        ) => await _inner.GetByUniqueAsync(uniquePredicate, selector, include);

        public virtual async Task<dynamic?> GetDynamicAsync(
            string fields,
            Expression<Func<T, bool>>? filter = null
        ) => await _inner.GetDynamicAsync(fields, filter);

        public virtual async Task SoftDeleteAsync(Guid id) => await _inner.SoftDeleteAsync(id);

        public virtual async Task SoftDeleteRangeAsync(Expression<Func<T, bool>>? filter = null) =>
            await _inner.SoftDeleteRangeAsync(filter);

        public virtual async Task<T> UpdateAsync(T entity) => await _inner.UpdateAsync(entity);

        public virtual async Task<List<T>> UpdateAsync(
            Expression<Func<T, bool>>? where,
            Action<T> update
        ) => await _inner.UpdateAsync(where, update);

        public virtual async Task<T?> UpdateAsync<TKey>(
            Expression<Func<T, TKey>> keySelector,
            TKey key,
            Action<T> update
        ) => await _inner.UpdateAsync(keySelector, key, update);

        public virtual async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities) =>
            await _inner.UpdateRangeAsync(entities);
    }
}
