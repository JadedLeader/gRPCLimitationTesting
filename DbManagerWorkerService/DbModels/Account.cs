using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbManagerWorkerService.DbModels
{
    public class Account
    {

        [Key]
        public Guid AccountUnique { get; set; }

        [ForeignKey("AuthUnique")]
        public Guid? AuthUnique { get; set; }

        public string Username { get; set; }    

        public string Password { get; set; }

        public string? TimeOfLogin { get; set; }

        public string TimeOfAccountCreation { get;set; }

        public string Role { get; set; }     

        //nav property to the token table
        public AuthToken? AuthToken { get; set; }

        //nav property to the session table
        public Session? Session { get; set; }



    }
}
