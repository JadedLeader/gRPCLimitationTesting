using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationStuff.DbModels
{
    public class AuthToken
    {

        [Key]
        public Guid AuthUnique { get; set; }

        public string? CurrentToken { get; set; }

        public string? RefreshToken { get; set; }    



    }
}
