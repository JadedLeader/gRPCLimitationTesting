using ConfigurationStuff.DbModels;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationStuff
{
    public interface IDataContexts
    {

        public DbSet<Account> Account { get; set; }
        public DbSet<AuthToken> AuthToken { get; set; }
        public DbSet<ClientInstance> ClientInstance { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<DelayCalc> DelayCalc { get; set; }
        
        public DbSet<SessionRuns> SessionRuns { get; set; }
        
        public DbSet<LatencyMeasurements> LatencyMeasurements { get; set; }

     
    }
}
