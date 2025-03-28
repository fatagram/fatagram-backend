using Fatagram.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.User
{
    public class UpdateUserPrivacyDto
    {
        public string Field { get; set; } = string.Empty;
        public PrivacyLevel PrivacyLevel { get; set; }
    }
}
