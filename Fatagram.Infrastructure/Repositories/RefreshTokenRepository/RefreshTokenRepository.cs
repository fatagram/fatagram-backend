using System.Linq.Expressions;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Repositories.RefreshTokenRepository
{
    /// <summary>
    /// Repository for managing refresh tokens.
    /// </summary>
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public RefreshTokenRepository(AppDbContext dbContext)
            : base(dbContext) { }
    }
}
