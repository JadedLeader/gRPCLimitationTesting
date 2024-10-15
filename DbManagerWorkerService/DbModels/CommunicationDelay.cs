using System.ComponentModel.DataAnnotations;

namespace DbManagerWorkerService.DbModels
{
    public class CommunicationDelay
    {


        [Key]
        
        public int Id { get; set; }

        public string DelayGuid { get; set; }

        public string CommunicationType { get; set; }

        public string RequestType { get; set; }

        public int DataLength { get; set; }

        public TimeSpan Delay { get; set; }



    }
}
