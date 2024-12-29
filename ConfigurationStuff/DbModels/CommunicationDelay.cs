using System.ComponentModel.DataAnnotations;

namespace ConfigurationStuff.DbModels
{
    public class CommunicationDelay
    {


        [Key]
        
        public int Id { get; set; }

        public string ClientId { get; set; }

        public string MessageDelayId { get; set; }

        public string CommunicationType { get; set; }

        public string RequestType { get; set; }

        public int DataIterations { get; set; }
        
        public string DataContents { get; set; }

        public TimeSpan Delay { get; set; }



    }
}
