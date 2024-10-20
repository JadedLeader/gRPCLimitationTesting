using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCommonalities.ReturnModels.ReturnTypes
{
    public class RawTimingValue
    {


        public string? RequestType { get; set; }

        public DateTime Timestamp { get; set; }

        public string RequestId { get; set; }

    }
}
