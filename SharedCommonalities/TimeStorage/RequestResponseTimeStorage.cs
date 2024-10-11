using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.ReturnModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharedCommonalities.TimeStorage
{
    public class RequestResponseTimeStorage 
    {
        //stores the client request timing, key of the GUID and string of all the other information

        //might adapt this to take in a custom type with the streaming type and the date time as string, leave the key as string guid
        public static Dictionary<string, UnaryInfo> _ClientRequestTiming = new Dictionary<string, UnaryInfo>();

        //largely the same as the client
        public static Dictionary<string, UnaryInfo> _ServerResponseTiming = new Dictionary<string, UnaryInfo>();

        public static Dictionary<string, UnaryInfo> _ActualDelayCalculations = new Dictionary<string, UnaryInfo>();

        public RequestResponseTimeStorage()
        {
            Console.WriteLine("New instance of RequestResponseTimeStorage created.");
        }

        public static void RemoveDelayCalulcationFromDict(string guid)
        {
            _ClientRequestTiming.Remove(guid);
        }

        public static void AddFinalTimeToDict(string guid, UnaryInfo calculatedTime)
        {
            _ActualDelayCalculations.Add(guid, calculatedTime);
        }

        public static void AddRequestToList(string requestKey, UnaryInfo unaryRequestInfo)
        {

           _ClientRequestTiming.Add(requestKey, unaryRequestInfo);

            Console.WriteLine($"added to request list: request key {requestKey}");
        }

        public static void AddResponseToList(string responseKey, UnaryInfo unaryResponseInfo)
        {

            _ServerResponseTiming.Add(responseKey, unaryResponseInfo);
            
        }

        private static void RemovingOldTimesFromDicts(string guidKey)
        {

            _ClientRequestTiming.Remove(guidKey);
            _ServerResponseTiming.Remove(guidKey);
        }

        public static Dictionary<string, UnaryInfo> ReturnDelayCalculations()
        {
            return _ActualDelayCalculations;
        }

        public static Dictionary<string, UnaryInfo> ReturnClientRequests()
        {
            return _ClientRequestTiming; 
        }

        public static Dictionary<string, UnaryInfo> ReturnServerResponse()
        {
            return _ServerResponseTiming;
        }

        /// <summary>
        /// Takes in a Guid, retrieves that client request based on the key from the "ClientRequestTiming" dictionary
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>A RawTimingValue that will contain the ID of the request, the timestamp and the request type</returns>
        /// <exception cref="Exception"></exception>
        public static RawTimingValue GettingClientRequestViaGuid(string guid)
        {
            var clientRequest = _ClientRequestTiming.FirstOrDefault(x => x.Key == guid);    

            if(clientRequest.Key == null || clientRequest.Value == null)
            {
                throw new Exception($"Either a client request key or a client request value is null here");
            }

            var clientReturn = new RawTimingValue()
            {
                RequestId = clientRequest.Key,
                Timestamp = clientRequest.Value.TimeOfRequest.Value,
                RequestType = "Unary"
            };

            return clientReturn;
        }

        /// <summary>
        /// Takes in a guid, retrieves the client response based on the key from the "SeverResponseTiming" dictionary
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>A RawTimingValue type that stores the request ID, the timestamp and the request type</returns>
        /// <exception cref="Exception"></exception>
        public static RawTimingValue GettingServerResponseViaGuid(string guid)
        {
            var serverResponse = _ServerResponseTiming.FirstOrDefault(x => x.Key == guid);

            if(serverResponse.Key == null || serverResponse.Value == null)
            {
                throw new Exception("Either a server response key or a server response value is null here");
            }

            var serverReturn = new RawTimingValue()
            {
                RequestId = serverResponse.Key,
                Timestamp = serverResponse.Value.TimeOfRequest.Value,
                RequestType = "Unary"
            }; 

            return serverReturn;
        }

       
    }
}
