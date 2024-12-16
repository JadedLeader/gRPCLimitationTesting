using SharedCommonalities.Abstracts;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.ReturnModels.ReturnTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharedCommonalities.TimeStorage
{
    public class RequestResponseTimeStorage : DictionariesAbstract<string, UnaryInfo>
    {
        //stores the client request timing, key of the GUID and string of all the other information

        //might adapt this to take in a custom type with the streaming type and the date time as string, leave the key as string guid
        public Dictionary<string, UnaryInfo> _ClientRequestTiming = new Dictionary<string, UnaryInfo>();

        //largely the same as the client
        public Dictionary<string, UnaryInfo> _ServerResponseTiming = new Dictionary<string, UnaryInfo>();
        
        //the string here currently just states the message ID, not the overarching ID which is what we'll require to store the new record into the DB
        //to do this we'll have to ammend the client request timing and the server response timing keys to also contain the overaching ID
        public Dictionary<string, UnaryInfo> _ActualDelayCalculations = new Dictionary<string, UnaryInfo>();

        public RequestResponseTimeStorage()
        {
            Console.WriteLine("New instance of RequestResponseTimeStorage created.");
        }

        public override void AddToDictionary(Dictionary<string, UnaryInfo> dictionaryName, string dataToAddKey, UnaryInfo dataToAddValue)
        {
            base.AddToDictionary(dictionaryName, dataToAddKey, dataToAddValue);
        }

        public override void RemoveFromDictionary(Dictionary<string, UnaryInfo> dictionaryName, string dataKey)
        {
            base.RemoveFromDictionary(dictionaryName, dataKey);
        }

        public override Dictionary<string, UnaryInfo> ReturnDictionary(Dictionary<string, UnaryInfo> dictionaryName)
        {
            return base.ReturnDictionary(dictionaryName);
        }

        /// <summary>
        /// Takes in a Guid, retrieves that client request based on the key from the "ClientRequestTiming" dictionary
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>A RawTimingValue that will contain the ID of the request, the timestamp and the request type</returns>
        /// <exception cref="Exception"></exception>
        public RawTimingValue GettingClientRequestViaGuid(string guid)
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
        public RawTimingValue GettingServerResponseViaGuid(string guid)
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
