using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fatagram.Domain.Models
{
    public class Localized : BaseEntity
    {
        public string LocalizationKey { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        // Navigation property
        public Language Language { get; set; } = null!;
    }
}
