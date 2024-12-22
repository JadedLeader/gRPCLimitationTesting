using DbManagerWorkerService.DbModels;
using DbManagerWorkerService.Interfaces.DataContext;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace DbManagerWorkerService.DbContexts
{
    public class DataContexts : DbContext, IDataContexts
    {
        private readonly IConfiguration _config;
        public DataContexts(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var connectionString = _config.GetConnectionString("DbConnection");

            if(connectionString == null)
            {
                Console.WriteLine($"no connection string passed");
            }

            optionsBuilder.UseSqlServer(connectionString);
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var account = modelBuilder.Entity<Account>();
            var authToken = modelBuilder.Entity<AuthToken>();
            var clientInstance = modelBuilder.Entity<ClientInstance>();
            var session = modelBuilder.Entity<Session>();
            var delayCalc  = modelBuilder.Entity<DelayCalc>();

          

         /*   account
                .HasOne(a => a.Session)
                .WithOne(s => s.Account)
                .HasForeignKey<Session>(s => s.AccountUnique); */

            //key 
           

            //props 
         
        }

        public async Task<int> SaveChangesAsync()
        {
             return await base.SaveChangesAsync();
        }

        //public DbSet<CommunicationDelay> CommunicationDelay { get; set; }

        public DbSet<Account> Account { get; set; }
        public DbSet<AuthToken> AuthToken { get; set; }
        public DbSet<ClientInstance> ClientInstance { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<DelayCalc> DelayCalc { get; set; }




    }
}
