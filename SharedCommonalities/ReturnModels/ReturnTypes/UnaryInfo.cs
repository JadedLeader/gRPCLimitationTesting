﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace SharedCommonalities.ReturnModels.ReturnTypes
{
    public class UnaryInfo
    {

        public DateTime? TimeOfRequest { get; set; }
        public TimeSpan? Delay { get; set; }
        public string? TypeOfData { get; set; }
        public int LengthOfData { get; set; }
        public string DataContents { get; set; }
        public string RequestType { get; set ; }
        public string DataContentSize { get; set; }
        public string? BatchRequestId { get; set; }
        public object? ClientInstance { get; set; }
        public string DataIterations { get; set; }


    }
}
