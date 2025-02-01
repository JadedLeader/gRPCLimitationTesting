using Serilog;
namespace gRPCToolFrontEnd.Services
{
    public class ThroughputService
    {

        private readonly Throughput.ThroughputClient _throughputClient;

        public event Action<GetStreamingSingleThroughputResponse> OnStreamingSingleThroughput;

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

    }
}
