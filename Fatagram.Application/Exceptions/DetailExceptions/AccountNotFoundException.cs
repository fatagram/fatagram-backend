using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;

namespace Fatagram.Application.Exceptions
{
    public class AccountNotFoundException : NotFoundException
    {
        public AccountNotFoundException()
            : base("ACCOUNT_NOT_FOUND", "Account not found") { }
    }
}
