using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.UserServices.UserConfigServices.Interfaces
{
    public interface IUserConfigService
    {
        Task<Result<string>> ChangeLanguage(Guid userId, string langCode);
    }
}
