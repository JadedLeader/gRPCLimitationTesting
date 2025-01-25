using Serilog;

namespace gRPCToolFrontEnd.LocalStorage
{
    public class ClientStorage
    {


        public int TotalUnaryClients { get; set; }

        public int TotalStreamingClients { get; set; }

        public event Action<int> OnUnaryClientsUpdated;
        public event Action<int> OnStreamingClientUpdated;

        public ClientStorage()
        {
            
        }

        public void IncrementUnaryClients()
        {
            Log.Information($"Unary client has been incremented");

            TotalUnaryClients += 1;

            OnUnaryClientsUpdated?.Invoke(TotalUnaryClients);
        }

        public void IncrementStreamingClients()
        {
            Log.Information($"Streaming client has been incremented");

            TotalStreamingClients += 1;

            OnStreamingClientUpdated?.Invoke(TotalStreamingClients);
        }

        public int GetTotalUnaryClients()
        {
            return TotalUnaryClients;
        }

        public int GetTotalStreamingClients()
        {
            return TotalStreamingClients;
        }

    }
}
