using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions
{
    public class AccountNotFoundException : AppException
    {
        public AccountNotFoundException() : base("ACCOUNT_NOT_FOUND", "Account not found")
        { }
    }
}
