using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbManagerWorkerService.DbModels
{
    public class Session
    {

        [Key]
        public Guid SessionUnique { get; set; }

        [ForeignKey("AccountUnique")]
        public Guid AccountUnique { get; set; } 

        public string SessionCreated { get; set; }

        public ClientInstance ClientInstance { get; set; }


    }
}
