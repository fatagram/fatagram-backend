using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Fatagram.Domain.Enums;

namespace Fatagram.Domain.Models.UserInformations
{
    [Table("user_school")]
    public class UserSchool : BaseEntity
    {
        [Column("id", TypeName = "uuid")]
        [Key]
        public Guid Id { get; set; }

        [Column("user_id", TypeName = "uuid")]
        public Guid UserId { get; set; }

        [Column("school_id", TypeName = "uuid")]
        public Guid SchoolId { get; set; }

        [Column("start", TypeName = "timestamptz")]
        public DateTime Start { get; set; }

        [Column("end", TypeName = "timestamptz")]
        public DateTime End { get; set; }

        [Column("school_state", TypeName = "varchar(20)")]
        public SchoolState SchoolState { get; set; }
    }
}
