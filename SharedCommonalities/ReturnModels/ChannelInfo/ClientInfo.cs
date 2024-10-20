using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCommonalities.ReturnModels.ChannelInfo
{
    public class ClientInfo
    {

        public Guid ClientId { get; set; }

        public object Message { get; set; }

        public bool IsActiveClient { get; set; }

    }
}
