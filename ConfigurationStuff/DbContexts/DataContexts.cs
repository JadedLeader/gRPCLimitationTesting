using ConfigurationStuff.DbModels;
using DbManagerWorkerService.Interfaces.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Principal;

namespace ConfigurationStuff.DbContexts
{
    public class DataContexts : DbContext, IDataContexts
    {
        private readonly IConfiguration _config;
        public DataContexts(DbContextOptions<DataContexts> options, IConfiguration config) : base(options)
        {
            _config = config;

            Console.WriteLine($"DB CONTEXT INSTANCE CREATED : {this.GetHashCode()}");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var connectionString = _config.GetConnectionString("DbConnection");

            if(connectionString == null)
            {
                Console.WriteLine($"no connection string passed");
            }

            object value = optionsBuilder.UseSqlServer(connectionString);
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var account = modelBuilder.Entity<Account>();
            var authToken = modelBuilder.Entity<AuthToken>();
            var clientInstance = modelBuilder.Entity<ClientInstance>();
            var session = modelBuilder.Entity<Session>();
            var delayCalc  = modelBuilder.Entity<DelayCalc>();

            clientInstance
            .HasOne<Session>() // Specify the principal entity
            .WithMany(s => s.ClientInstance) // Specify the navigation property
            .HasForeignKey(ci => ci.SessionUnique) // Specify the foreign key
            .OnDelete(DeleteBehavior.Cascade);

            /*   account
                   .HasOne(a => a.Session)
                   .WithOne(s => s.Account)
                   .HasForeignKey<Session>(s => s.AccountUnique); */

            clientInstance.Property(a => a.ClientUnique).ValueGeneratedNever();


            //key 


            //props 

        }

        

        //public DbSet<CommunicationDelay> CommunicationDelay { get; set; }

        public DbSet<Account> Account { get; set; }
        public DbSet<AuthToken> AuthToken { get; set; }
        public DbSet<ClientInstance> ClientInstance { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<DelayCalc> DelayCalc { get; set; }




    }
}
