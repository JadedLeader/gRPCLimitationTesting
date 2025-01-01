namespace gRPCToolFrontEnd.LocalStorage
{
    public class AccountDetailsStore
    {


        public string Username { get; set; }

        public string SessionUnique { get; set; }
        public AccountDetailsStore(string username, string sessionUnique)
        {
            Username = username;
            SessionUnique = sessionUnique;
        }

    }
}
