using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces
{
    public interface IBaseRepository<TEntity>
        where TEntity : BaseEntity
    {
        Task<TResult?> GetAsync<TResult>(
            Guid id,
            Expression<Func<TEntity, TResult>>? selector = null
        );

        Task<List<TResult>> GetAllAsync<TResult, TKey>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, TKey>>? orderBy = null,
            bool orderDesc = false,
            int? limit = null,
            TKey? lastKey = default,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null
        )
            where TKey : struct, IComparable<TKey>;

        Task<List<TResult>> GetAllAsync<TResult, TKey>(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, TKey>>? orderBy = null,
            bool orderDesc = false,
            int? limit = null,
            TKey? lastKey = default,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null
        )
            where TKey : struct, IComparable<TKey>;

        Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null);

        Task<dynamic?> GetDynamicAsync(
            string fields,
            Expression<Func<TEntity, bool>>? filter = null
        );

        Task<List<TResult>> GetAllAsync<TResult>(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, TResult>>? selector = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null
        );

        Task<TEntity> AddAsync(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities);

        Task<List<TEntity>> UpdateAsync(
            Expression<Func<TEntity, bool>>? where,
            Action<TEntity> update
        );

        Task<TEntity?> UpdateAsync<TKey>(
            Expression<Func<TEntity, TKey>> keySelector,
            TKey key,
            Action<TEntity> update
        );

        Task DeleteAsync(Guid id);

        Task DeleteAsync(TEntity entity);

        Task SoftDeleteAsync(Guid id);

        Task SoftDeleteRangeAsync(Expression<Func<TEntity, bool>>? filter = null);
    }
}
