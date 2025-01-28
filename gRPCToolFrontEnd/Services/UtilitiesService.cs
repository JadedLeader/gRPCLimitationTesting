using Grpc.Core;
using gRPCToolFrontEnd.DictionaryModel;
using gRPCToolFrontEnd.LocalStorage;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System.Collections.Concurrent;
using System.Reactive.Subjects;

namespace gRPCToolFrontEnd.Services
{
    public class UtilitiesService : IDisposable
    {

        private readonly RequestDelayStorage _requestDelayStorage;

        private readonly Utilities.UtilitiesClient _utilitiesClient;
        private CancellationTokenSource _cancellationToken;

        public event Action<GetClientsWithMessagesResponse> OnUpdateReceived;
        public event Action<GetStreamingBatchDelaysResponse> OnBatchReceived;
        public event Action<GetStreamingDelaysResponse> OnStreamingSingleReceived;
        public event Action<GetUnaryDelaysResponse> OnUnarySingleReceived;
        public event Action<GetUnaryBatchDelaysResponse> OnUnaryBatchReceived;

        public event Action<GetBestThroughputResponse> OnBestThroughputReceived;

        public UtilitiesService(Utilities.UtilitiesClient utilitiesClient)
        {
            _utilitiesClient = utilitiesClient;
            
        }

        public void StartReceivingMessages(GetClientsWithMessagesRequest request, string sessionUnique)
        {
            _cancellationToken = new CancellationTokenSource();

            Task.Run(() => ReceivingMessageStream(request, _cancellationToken.Token, sessionUnique));
        }

        public void StartReceivingStreamingBatchMessages(GetStreamingBatchDelaysRequest request, string sessionUnique)
        {
            _cancellationToken = new CancellationTokenSource();

            Task.Run(() => ReceivingStreamingBatchStream(request, _cancellationToken.Token, sessionUnique));
        }

        public void StartReceivingStreamingMessages(GetStreamingDelaysRequest streamingRequest, string sessionUnique)
        {
            _cancellationToken = new CancellationTokenSource();

            Task.Run(() => ReceivingStreamingSingleStream(streamingRequest, _cancellationToken.Token, sessionUnique));
        }

        public void StartReceivingUnaryMessages(GetUnaryDelaysRequest unaryRequest, string sessionUnique)
        {
            _cancellationToken = new CancellationTokenSource();

            Task.Run(() =>  ReceivingUnaryStream(unaryRequest, _cancellationToken.Token, sessionUnique));

        }

        public void StartReceivingUnaryBatchMessages(GetUnaryBatchDelaysRequest unaryBatchRequest, string sessionUnique)
        {
            _cancellationToken = new CancellationTokenSource();

            Task.Run(() => ReceivingUnaryBatchStream(unaryBatchRequest, _cancellationToken.Token, sessionUnique));
        }

        public void StartReceivingBestThroughput(GetBestThroughputRequest bestThroughputRequest)
        {
            _cancellationToken = new CancellationTokenSource();

            Task.Run(() => ReceivingBestThroughput(bestThroughputRequest, _cancellationToken.Token));
        }

        public void StopReceivingMessages()
        {
            if(_cancellationToken != null && !_cancellationToken.IsCancellationRequested)
            {
                _cancellationToken.Cancel();
            }
        }

        public async Task ReceivingBestThroughput(GetBestThroughputRequest bestThroughputRequest, CancellationToken cancellationToken)
        {
            Log.Information($"Started receiving best through from gRPC stream");

            using var call = _utilitiesClient.GetBestThroughput(bestThroughputRequest);

            while(await call.ResponseStream.MoveNext(cancellationToken))
            {
                var response = call.ResponseStream.Current;

               
                OnBestThroughputReceived?.Invoke(response);

            }
        }

        public async Task ReceivingMessageStream(GetClientsWithMessagesRequest messageRequest, CancellationToken cancellationToken, string sessionUnique)
        {
            Log.Information("Started receiving messages from gRPC stream.");

            try
            {
                Metadata metaData = new Metadata();

                metaData.Add("session-unique", sessionUnique);

                using var call = _utilitiesClient.GetClientsWithMessages(messageRequest, metaData);

                while (await call.ResponseStream.MoveNext(cancellationToken))
                {
                    var response = call.ResponseStream.Current;

                    Log.Debug($"Received update for ClientUnique: {response.ClientUnique}");
                    OnUpdateReceived?.Invoke(response);
                }
                
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while reading the gRPC stream.");
                throw;
            }
        }

        public async Task ReceivingStreamingBatchStream(GetStreamingBatchDelaysRequest batchStreamingRequest, CancellationToken cancellationToken, string sessionUnique)
        {
            Log.Information($"Started receiving messages from streaming batch utilities endpoint");

            try
            {
                Metadata metaData = new Metadata();

                metaData.Add("session-unique", sessionUnique);

                using var call = _utilitiesClient.GetstreamingBatchDelays(batchStreamingRequest, metaData);

                while(await call.ResponseStream.MoveNext(cancellationToken))
                {
                    var response = call.ResponseStream.Current;

                    Log.Information($"streaming batch request received, message ID: {response.GatheringStreamingBatchDelays.MessageId} with delay : {response.GatheringStreamingBatchDelays.Delay}");

                   

                    OnBatchReceived?.Invoke(response);
                }
                    
            }
            catch(Exception ex)
            {
                Log.Error($"{ex} Error wile reading the streaming batch stream");
                throw;
            }
        }

        public async Task ReceivingStreamingSingleStream(GetStreamingDelaysRequest batchStreamingRequest, CancellationToken cancellationToken, string sessionUnique)
        {

            Metadata metaData = new Metadata();

            metaData.Add("session-unique", sessionUnique);

            using var call = _utilitiesClient.GetStreamingDelays(batchStreamingRequest, metaData);

            while (await call.ResponseStream.MoveNext(cancellationToken))
            {

                var response = call.ResponseStream.Current;

                Log.Information($"Streaming single request received, message ID {response.GatheringStreamingDelays.MessageId} with delay : {response.GatheringStreamingDelays.Delay}");

                OnStreamingSingleReceived?.Invoke(response);
            }
            
        }

        public async Task ReceivingUnaryStream(GetUnaryDelaysRequest unaryRequest, CancellationToken cancellationToken, string sessionUnique)
        {
            Metadata metaData = new Metadata();

            metaData.Add("session-unique", sessionUnique); 

            using var call = _utilitiesClient.GetUnaryDelays(unaryRequest, metaData);

            while(await call.ResponseStream.MoveNext(cancellationToken))
            {
                var response = call.ResponseStream.Current;

                Log.Information($"Unary single request received, message ID {response.GatheringUnaryDelays.MessageId} with delay : {response.GatheringUnaryDelays.Delay}"); 

                OnUnarySingleReceived?.Invoke(response);
            }
        }

        public async Task ReceivingUnaryBatchStream(GetUnaryBatchDelaysRequest unaryBatchRequests, CancellationToken cancellationToken, string sessionUnique)
        {
            Metadata metadata = new Metadata();

            metadata.Add("session-unique", sessionUnique);

            using var call = _utilitiesClient.GetUnaryBatchDelays(unaryBatchRequests, metadata);

            while(await call.ResponseStream.MoveNext(cancellationToken))
            {
                var response = call.ResponseStream.Current;

                Log.Information($"Unary batch request received, message ID {response.GatheringUnaryBatchDelays} with delay : {response.GatheringUnaryBatchDelays.Delay}");

                OnUnaryBatchReceived?.Invoke(response);
            }
        }

        public void Dispose()
        {
            StopReceivingMessages();
            _cancellationToken?.Dispose();
        }

    }

}
    

