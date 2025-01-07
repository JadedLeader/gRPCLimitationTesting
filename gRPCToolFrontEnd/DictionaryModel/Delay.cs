namespace gRPCToolFrontEnd.DictionaryModel
{
    public class Delay
    {


        public string MessageId { get; set; }

        public string RequestType { get; set; }

        public string CommunicationType { get; set; }

        public int DataIterations { get; set; }
        public string DataContent { get; set; }

        public TimeSpan MessageDelay { get; set; }

    }
}
