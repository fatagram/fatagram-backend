using Fatagram.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Domain.Models.UserInformations
{
    [Table("user_job")]
    public class UserJob
    {
        [Column("id", TypeName = "uuid")]
        [Key]     
        public Guid Id { get; set; }

        [Column("user_id", TypeName = "uuid")]
        public Guid UserId { get; set; }

        [Column("job_id", TypeName = "uuid")]
        public Guid JobId { get; set; }

        [Column("start", TypeName = "timestamptz")]
        public DateTime Start { get; set; }

        [Column("end", TypeName = "timestamptz")]
        public DateTime End { get; set; }

        [Column("job_state", TypeName = "user_job_state")]
        public JobState JobState { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;
        public Job Job { get; set; } = null!;

    }
}
