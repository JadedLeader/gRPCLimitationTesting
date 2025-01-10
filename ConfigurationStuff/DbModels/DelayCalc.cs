using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationStuff.DbModels
{
    public class DelayCalc
    {

        [Key]
        public Guid messageId { get; set; }

        [ForeignKey("ClientUnique")]
        public Guid? ClientUnique { get; set; }

        public string? RequestType { get; set; }

        public string? CommunicationType { get; set; }

        public int DataIterations { get; set; } 

        public string? DataContent { get; set; }

        public TimeSpan? Delay { get ; set; }

        public DateTime RecordCreation { get; set; }  

        public ClientInstance? ClientInstance { get; set; }  

    }
}
