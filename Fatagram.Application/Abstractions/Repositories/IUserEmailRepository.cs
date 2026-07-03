using System.Threading.Tasks;
using Fatagram.Domain.Models;

namespace Fatagram.Application.Abstractions.Repositories
{
    public interface IUserEmailRepository : IBaseRepository<UserEmail>
    {
        Task<bool> IsEmailInUseAsync(string email);
    }
}
