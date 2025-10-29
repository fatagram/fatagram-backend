using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Dtos.User.Update
{
    public class ChangeUrlNameDto
    {
        public string UrlName { get; set; } = null!;
    }
}
