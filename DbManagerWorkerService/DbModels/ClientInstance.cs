using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbManagerWorkerService.DbModels
{
    public class ClientInstance
    {

        [Key]
        public Guid ClientUnique { get; set; }

        [ForeignKey("SessionUnique")]
        public Guid SessionUnique { get; set; } 

        public ICollection<DelayCalc> DelayCalcs { get; set; }


    }
}
