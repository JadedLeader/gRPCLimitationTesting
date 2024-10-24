using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCommonalities.ReturnModels.ReturnTypes
{
    public class ClientDetails
    {

        public Guid RequestId { get; set; }

        public long MessageLength { get; set; }

        public bool IsActiveClient { get; set; }

    }
}
