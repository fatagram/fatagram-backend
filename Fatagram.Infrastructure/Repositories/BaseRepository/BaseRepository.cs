using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

namespace Fatagram.Infrastructure.Repositories.BaseRepository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : BaseEntity
    {
        protected readonly AppDbContext _dbContext;
        protected readonly ILogger<BaseRepository<TEntity>>? _logger;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(
            AppDbContext dbContext,
            ILogger<BaseRepository<TEntity>>? logger = null
        )
        {
            _dbContext = dbContext;
            _logger = logger;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            await _dbSet.Where(e => e.Id == id).ExecuteDeleteAsync();
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<TResult>> GetAllAsync<TResult, TKey>(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, TResult>>? selector = null,
            Expression<Func<TEntity, TKey>>? orderBy = null,
            bool orderDesc = false,
            int? page = null,
            int? pageSize = null,
            TKey? lastKey = default
        )
        {
            var query = _dbSet.AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            if (lastKey != null && orderBy != null)
            {
                var param = orderBy.Parameters[0];
                var member = orderBy.Body;

                Expression comparison = orderDesc
                    ? Expression.LessThan(member, Expression.Constant(lastKey))
                    : Expression.GreaterThan(member, Expression.Constant(lastKey));
                var lambda = Expression.Lambda<Func<TEntity, bool>>(comparison, param);
                query = query.Where(lambda);
            }

            if (orderBy != null)
            {
                query = orderDesc ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            }

            if (page != null && pageSize != null)
            {
                query = query.Skip(((int)page - 1) * (int)pageSize).Take((int)pageSize);
            }

            if (selector != null)
            {
                return await query.Select(selector).ToListAsync();
            }
            else
            {
                if (typeof(TResult) == typeof(TEntity))
                {
                    var entities = await query.ToListAsync();
                    _logger?.LogInformation("Entities count: {Count}", entities.Count);
                    return [.. entities.Cast<TResult>()];
                }
                else
                {
                    throw new InvalidOperationException(
                        "Selector must be provided when TResult is not TEntity."
                    );
                }
            }
        }

        public Task<List<TResult>> GetAllAsync<TResult>(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, TResult>>? selector = null
        ) => GetAllAsync<TResult, object>(filter, selector);

        public async Task<TResult?> GetAsync<TResult>(
            Guid id,
            Expression<Func<TEntity, TResult>>? selector = null
        )
        {
            var query = _dbSet.AsQueryable();

            if (selector != null)
            {
                return await query.Where(e => e.Id == id).Select(selector).FirstOrDefaultAsync();
            }
            else
            {
                if (typeof(TResult) == typeof(TEntity))
                {
                    var entity = await query.FirstOrDefaultAsync(e => e.Id == id);
                    if (entity == null)
                        return default;
                    return (TResult)(object)entity;
                }
                else
                {
                    throw new InvalidOperationException(
                        "Selector must be provided when TResult is not TEntity."
                    );
                }
            }
        }

        public async Task<TResult?> GetByUniqueKeyAsync<TResult, TKey>(
            Expression<Func<TEntity, TKey?>> keySelector,
            TKey? key,
            Expression<Func<TEntity, TResult>>? selector = null
        )
        {
            var parameter = keySelector.Parameters[0];
            var body = Expression.Equal(keySelector.Body, Expression.Constant(key));
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

            if (selector != null)
            {
                return await _dbSet.Where(lambda).Select(selector).FirstOrDefaultAsync();
            }
            else
            {
                if (typeof(TResult) == typeof(TEntity))
                {
                    var entity = await _dbSet.FirstOrDefaultAsync(lambda);
                    if (entity == null)
                        return default;
                    return (TResult)(object)entity;
                }
                else
                {
                    throw new InvalidOperationException(
                        "Selector must be provided when TResult is not TEntity."
                    );
                }
            }
        }

        public async Task<dynamic?> GetDynamicAsync(
            string fields,
            Expression<Func<TEntity, bool>>? filter = null
        )
        {
            var query = _dbSet.AsQueryable();
            if (filter != null)
                query = query.Where(filter);
            return await query.Select($"new ({fields})").Cast<dynamic>().FirstOrDefaultAsync();
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            await _dbSet
                .Where(e => e.Id == id)
                .ExecuteUpdateAsync(e => e.SetProperty(p => p.DeletedAt, DateTime.UtcNow));
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<List<TEntity>> UpdateAsync(
            Expression<Func<TEntity, bool>>? where,
            Action<TEntity> update
        )
        {
            var query = _dbSet.AsQueryable();
            if (where != null)
            {
                query = query.Where(where);
            }
            var entities = await query.ToListAsync();
            foreach (var entity in entities)
            {
                update(entity);
            }
            await _dbContext.SaveChangesAsync();
            return entities;
        }

        public async Task<TEntity?> UpdateAsync<TKey>(
            Expression<Func<TEntity, TKey>> keySelector,
            TKey key,
            Action<TEntity> update
        )
        {
            var parameter = keySelector.Parameters[0];
            var body = Expression.Equal(keySelector.Body, Expression.Constant(key));
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

            var entity = await _dbSet.Where(lambda).FirstOrDefaultAsync();
            if (entity != null)
            {
                update(entity);
                await _dbContext.SaveChangesAsync();
                return entity;
            }
            return default;
        }
    }
}
