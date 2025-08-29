using Fatagram.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Domain.Models
{
    [Table("localized")]
    public class Localized
    {
        [Column("id", TypeName = "uuid")]
        public Guid Id { get; set; }

        [Column("localization_key", TypeName = "varchar(100)")]
        public string LocalizationKey { get; set; } = string.Empty;

        [Column("language_code", TypeName = "varchar(2)")]
        public string LanguageCode { get; set; } = string.Empty;

        [Column("value", TypeName = "text")]
        public string Value { get; set; } = string.Empty;

        // Navigation property
        public Language Language { get; set; } = null!;
    }
}
