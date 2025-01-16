using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationStuff.DbModels
{
    public class ClientInstance
    {

        [Key]
        public Guid ClientUnique { get; set; }

        public Guid? SessionUnique { get; set; } 

        public ICollection<DelayCalc?> DelayCalcs { get; set; } = new List<DelayCalc>();

        [Timestamp]
        public byte[] RowVersion { get; set; }


    }
}
