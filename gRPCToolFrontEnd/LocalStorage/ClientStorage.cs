namespace gRPCToolFrontEnd.LocalStorage
{
    public class ClientStorage
    {


        public int TotalUnaryClients { get; set; }

        public int TotalStreamingClients { get; set; }

        public ClientStorage()
        {
            
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
