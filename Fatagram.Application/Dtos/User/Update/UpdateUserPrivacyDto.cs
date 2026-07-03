using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Enums;

namespace Fatagram.Application.Dtos.User.Update
{
    public record UpdateUserPrivacyDto
    {
        public string Field { get; set; } = string.Empty;
        public PrivacyLevel PrivacyLevel { get; set; }
    }
}
