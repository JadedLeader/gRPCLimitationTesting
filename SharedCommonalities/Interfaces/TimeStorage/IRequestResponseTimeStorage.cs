using SharedCommonalities.ReturnModels;
using SharedCommonalities.TimeStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCommonalities.Interfaces.TimeStorage
{
    public interface IRequestResponseTimeStorage
    {

        public void AddRequestToList(string requestKey, UnaryInfo unaryRequestInfo);

        public  void AddResponseToList(string responseKey, UnaryInfo unaryResponseInfo);

        public Dictionary<string, UnaryInfo> ReturnClientRequests();

        public Dictionary<string, UnaryInfo> ReturnServerResponse();

        public RawTimingValue GettingClientRequestViaGuid(string guid);
        public RawTimingValue GettingServerResponseViaGuid(string guid);

        public Dictionary<string, UnaryInfo> ReturnDelayCalculations();

        public void AddFinalTimeToDict(string guid, UnaryInfo calculatedTime);

        public void RemoveDelayCalulcationFromDict(string guid);


    }
}
