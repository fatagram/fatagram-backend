using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Fatagram.Domain.Models;

namespace Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces
{
    public interface IBaseRepository<TEntity>
        where TEntity : BaseEntity
    {
        Task<TResult?> GetAsync<TResult>(
            Guid id,
            Expression<Func<TEntity, TResult>>? selector = null
        );
        Task<TResult?> GetByUniqueKeyAsync<TResult, TKey>(
            Expression<Func<TEntity, TKey>> keySelector,
            TKey key,
            Expression<Func<TEntity, TResult>>? selector = null
        );
        Task<List<TResult>> GetAllAsync<TResult, TKey>(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, TResult>>? selector = null,
            Expression<Func<TEntity, TKey>>? orderBy = null,
            bool orderDesc = false,
            int? page = null,
            int? pageSize = null,
            TKey? lastKey = default
        );
        Task<dynamic?> GetDynamicAsync(
            string fields,
            Expression<Func<TEntity, bool>>? filter = null
        );
        Task<List<TResult>> GetAllAsync<TResult>(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, TResult>>? selector = null
        );
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(Guid id);
        Task SoftDeleteAsync(Guid id);
    }
}
