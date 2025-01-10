using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SharedCommonalities.ReturnModels.ReturnTypes
{
    public class ClientDetails
    {

        public Guid ClientUnique { get; set; }
        public Guid messageId { get; set; }

        public long MessageLength { get; set; }

        public string DataContent { get; set; }

        public bool IsActiveClient { get; set; }

        public string DataContentSize { get; set; }

    }
}
