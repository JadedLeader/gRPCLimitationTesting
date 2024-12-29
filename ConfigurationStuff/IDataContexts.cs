using ConfigurationStuff.DbModels;
using Microsoft.EntityFrameworkCore;


namespace DbManagerWorkerService.Interfaces.DataContext
{
    public interface IDataContexts
    {

        public DbSet<Account> Account { get; set; }
        public DbSet<AuthToken> AuthToken { get; set; }
        public DbSet<ClientInstance> ClientInstance { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<DelayCalc> DelayCalc { get; set; }

     
    }
}
