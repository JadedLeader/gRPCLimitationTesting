using Serilog;
using System.Threading;
namespace gRPCToolFrontEnd.Services
{
    public class ThroughputService : IDisposable
    {

        private readonly Throughput.ThroughputClient _throughputClient;

        public event Action<GetStreamingSingleThroughputResponse> OnStreamingSingleThroughput;
        public event Action<GetStreamingBatchThroughputResponse> OnStreamingBatchThroughput;
        public event Action<GetUnarySingleThroughputResponse> OnUnarySingleThroughput;
        public event Action<GetUnaryBatchThroughputResponse> OnUnaryBatchThroughput;

        private CancellationTokenSource _cancellationTokenSource;
        
        public ThroughputService(Throughput.ThroughputClient throughputClient)
        {
            _throughputClient = throughputClient;
        }

        public void StartReceivingStreamingSingleThroughput(GetStreamingSingleThroughputRequest streamingSingleRequest)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => HandleStreamingSingleThroughput(streamingSingleRequest, _cancellationTokenSource.Token));
        }

        public void StartReceivingStreamingBatchThroughput(GetStreamingBatchThroughputRequest streamingBatchRequest)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => HandleStreamingBatchThroughput(streamingBatchRequest, _cancellationTokenSource.Token));
        }

        public void StartReceivingUnarySingleThroughput(GetUnarySingleThroughputRequest unarySingleThroughput)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => HandleUnarySingleThroughput(unarySingleThroughput, _cancellationTokenSource.Token));
        }

        public void StartReceivingUnaryBatchThroughput(GetUnaryBatchThroughputRequest unaryBatchThroughput)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => HandleUnaryBatchThroughput(unaryBatchThroughput, _cancellationTokenSource.Token));
        }

        public async Task HandleUnaryBatchThroughput(GetUnaryBatchThroughputRequest unaryBatchThroughputRequest, CancellationToken cancellationToken)
        {
            using var call = _throughputClient.GetUnaryBatchThroughput(unaryBatchThroughputRequest); 

            while(await call.ResponseStream.MoveNext(cancellationToken))
            {
                var currentResponse = call.ResponseStream.Current;

                Log.Information($"Received unary batch throughput request: {currentResponse.CurrentThroughput}");

                OnUnaryBatchThroughput?.Invoke( currentResponse );
            }
        }

        public async Task HandleStreamingSingleThroughput(GetStreamingSingleThroughputRequest streamingSingleThroughput, CancellationToken cancellationToken)
        {

            using var call = _throughputClient.GetStreamingSingleThroughput(streamingSingleThroughput);

            while(await call.ResponseStream.MoveNext(cancellationToken))
            {
                var currentResponse = call.ResponseStream.Current;

                Log.Information($"received streaming single throughput request: {currentResponse.CurrentThroughput}");

                OnStreamingSingleThroughput?.Invoke(currentResponse);
            }
        }

        public async Task HandleStreamingBatchThroughput(GetStreamingBatchThroughputRequest streamingBatchThroughput, CancellationToken cancellationToken)
        {
            using var call = _throughputClient.GetStreamingBatchThroughput(streamingBatchThroughput);

            while(await call.ResponseStream.MoveNext(cancellationToken))
            {
                var currentResponse = call.ResponseStream.Current;

                Log.Information($"received streaming batch throughput request: {currentResponse.CurrentThroughput}"); 

                OnStreamingBatchThroughput?.Invoke(currentResponse);

            }
        }

        public async Task HandleUnarySingleThroughput(GetUnarySingleThroughputRequest unarySingleThroughput, CancellationToken cancellationToken)
        {
            using var call = _throughputClient.GetUnarySingleThroughput(unarySingleThroughput);

            while(await call.ResponseStream.MoveNext(cancellationToken))
            {
                var currentResponse = call.ResponseStream.Current;

                Log.Information($"received unary single throughput request: {currentResponse.CurrentThroughput}"); 

                OnUnarySingleThroughput?.Invoke(currentResponse);
            }
        }

        public void StopReceivingMessages()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public void Dispose()
        {
            StopReceivingMessages();

            _cancellationTokenSource?.Dispose();
        }
    }
}
