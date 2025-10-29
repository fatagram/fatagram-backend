using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Fatagram.Domain.Enums.NotificationServices;

namespace Fatagram.Domain.Models
{
    public class NotificationContent : BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public NotificationType Type { get; set; }

        [Required]
        [Column("lang_code", TypeName = "varchar(2)")]
        public string LanguageCode { get; set; } = string.Empty;

        [Required]
        [Column("content", TypeName = "text")]
        public string Content { get; set; } = string.Empty;

        public Language? Language { get; set; }
    }
}
