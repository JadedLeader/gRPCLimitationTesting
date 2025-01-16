namespace gRPCToolFrontEnd.Interfaces
{
    public interface IRequestTypeHandlingFactory
    {

        public IRequestReceiver RequestReceiver(bool isStreaming, string dropDownState, bool unaryOrBatch, Guid? channelUnique, string clientUnique, string fileSize,
            int iterationsInbatch, int amountOfRequests);

    }
}
