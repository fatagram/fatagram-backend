using Fatagram.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.User
{
    public class UserDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid? Id { get; set; }

        public string? Username { get; set; }

        public string? LastName { get; set; }

        public string? FirstName { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Bio { get; set; }

        public string? Avatar { get; set; }

        public string? Background { get; set; }

        public DateTime? BirthDay { get; set; }
    }
}
