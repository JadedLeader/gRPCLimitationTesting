using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationStuff.DbModels
{
    public class Session
    {

        [Key]
        public Guid SessionUnique { get; set; }

        public Guid? AccountUnique { get; set; }

        public string? SessionCreated { get; set; }

        public ICollection<ClientInstance?> ClientInstance = new List<ClientInstance>();


    }
}
