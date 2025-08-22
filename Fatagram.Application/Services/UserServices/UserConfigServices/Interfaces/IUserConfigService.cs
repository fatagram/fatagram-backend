using Fatagram.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Services.UserServices.UserConfigServices.Interfaces
{
    public interface IUserConfigService
    {
        Task<Result<string>> ChangeLanguage(string userId, string langCode);
    }
}
