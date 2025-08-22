using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class LanguageExtension
    {
        public static void AddLanguage(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Language>().HasData(
                new Language { Code = "en", Name = "English" },
                new Language { Code = "vi", Name = "Tiếng Việt" }
            );
        }
    }
}
