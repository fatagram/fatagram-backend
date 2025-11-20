using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;

namespace Fatagram.Infrastructure.Repositories.RefreshTokenRepository.Interface
{
    /// <summary>
    /// Interface for the refresh token repository
    /// </summary>
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken> { }
}
