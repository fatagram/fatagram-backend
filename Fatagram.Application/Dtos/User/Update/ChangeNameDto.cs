using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Dtos.User.Update
{
    public class ChangeNameDto
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;
    }
}
