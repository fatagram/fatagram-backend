using Fatagram.Application.Dtos.User;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Fatagram.Application.Dtos.Account
{
    /// <summary>
    /// Data Transfer Object for user registration.
    /// </summary>
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public UserDto User { get; set; } = new();
    }

    public class AccountsDto 
    {
        public List<AccountDto> Accounts { get; set; } = new();
        public int TotalPage { get; set; }
        public int TotalAccount { get; set; }

    }
}
