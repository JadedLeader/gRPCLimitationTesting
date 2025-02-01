using Serilog;

namespace gRPCToolFrontEnd.LocalStorage
{
    public class SentRequestStorage
    {

        public int SingleStreamingRequestsSent { get; set; }

        public int BatchStreamingRequestsSent { get; set; }

        public int SingleUnaryRequestsSent { get; set; }

        public int BatchUnaryRequestsSent { get; set; }

        public event Action<int> OnSingleStreamingRequestSent;

        public event Action<int> OnBatchStreamingRequestsSent;

        public event Action<int> OnSingleUnaryRequestsSent;

        public event Action<int> OnBatchUnaryRequestsSent;

        public SentRequestStorage()
        {

        }

        public void IncrementSingleStreamingRequest()
        {
            Log.Information($"INCREMENTING SINGLE STREAMING REQUEST");

            SingleStreamingRequestsSent += 1;

            OnSingleStreamingRequestSent?.Invoke(SingleStreamingRequestsSent);
        }

        public void IncrementBatchStreamingRequest(int batchNumber)
        {
            BatchStreamingRequestsSent += batchNumber;

            OnBatchStreamingRequestsSent?.Invoke(BatchStreamingRequestsSent);
        }

        public void IncrementSingleUnaryRequest()
        {
            SingleUnaryRequestsSent += 1;
            OnSingleUnaryRequestsSent?.Invoke(SingleUnaryRequestsSent);
        }

        public void IncrementBatchUnaryRequest(int batchNumber)
        {
            BatchUnaryRequestsSent += batchNumber;

            OnBatchUnaryRequestsSent?.Invoke(BatchUnaryRequestsSent);
        }



    }
}
