using Grpc.Core;
using gRPCToolFrontEnd.DictionaryModel;
using gRPCToolFrontEnd.LocalStorage;
using Serilog;
using System.Collections.Concurrent;
using System.Reactive.Subjects;

namespace gRPCToolFrontEnd.Services
{
    public class UtilitiesService
    {

        private readonly Utilities.UtilitiesClient _utilitiesClient;

        public UtilitiesService(Utilities.UtilitiesClient utilitiesClient)
        {
            _utilitiesClient = utilitiesClient;
        }

        public async Task ReceivingMessageStream(GetClientsWithMessagesRequest messageRequest, ConcurrentDictionary<Guid, List<Delay>> clientsWithMessagesDict, bool isUiUpdating, BehaviorSubject<Dictionary<Guid, List<Delay>>> behaviourSubject)
        {

            while (isUiUpdating)
            {
                using var call = _utilitiesClient.GetClientsWithMessages(messageRequest);

                await foreach (var currentResponse in call.ResponseStream.ReadAllAsync())
                {

                    Delay newDelayValue = new Delay
                    {
                        MessageId = currentResponse.MessageId,
                        RequestType = currentResponse.RequestType,
                        CommunicationType = currentResponse.CommunicationType,
                        DataIterations = currentResponse.DataIterations,
                        DataContent = currentResponse.Datacontent,
                        MessageDelay = TimeSpan.Parse(currentResponse.Delay),
                    };

                    //we need to do some form of key checking inside of here 
                    // if the key already exists in the dictionary, we want to add it to the list<delay> 
                    //if the key doesn't exist in the dictionary, then we add it normally

                    if (clientsWithMessagesDict.ContainsKey(Guid.Parse(currentResponse.ClientUnique)))
                    {
                        //Log.Information($"Key is already within the dictionary, adding this item to the delay list");

                        List<Delay> clientValue = clientsWithMessagesDict[Guid.Parse(currentResponse.ClientUnique)];

                        if (!clientValue.Any(delay => delay.MessageId == currentResponse.MessageId))
                        {
                            clientValue.Add(newDelayValue);
                        }
                        else
                        {
                            //Log.Warning($"Duplicate message ID detected: {currentResponse.MessageId}. Skipping...");
                        }
                    }
                    else
                    {
                        List<Delay> delayList = new List<Delay>();

                        delayList.Add(newDelayValue);
                        clientsWithMessagesDict.TryAdd(Guid.Parse(currentResponse.ClientUnique), delayList);
                    }

                    behaviourSubject.OnNext(new Dictionary<Guid, List<Delay>>(clientsWithMessagesDict));

                }
            } 

        }

    }
    
}
