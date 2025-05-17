using ConfigurationStuff.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace ConfigurationStuff.DbContexts
{
    public class DataContexts : DbContext, IDataContexts
    {
        private readonly IConfiguration _config;
        public DataContexts(DbContextOptions<DataContexts> options, IConfiguration config) : base(options)
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
            .HasOne<Session>() 
            .WithMany(s => s.ClientInstance) 
            .HasForeignKey(ci => ci.SessionUnique)
            .OnDelete(DeleteBehavior.Cascade);


            clientInstance.Property(a => a.ClientUnique).ValueGeneratedNever();
            clientInstance.Property(a => a.SessionUnique).ValueGeneratedNever();
            clientInstance.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            

        }

        public DbSet<Account> Account { get; set; }
        public DbSet<AuthToken> AuthToken { get; set; }
        public DbSet<ClientInstance> ClientInstance { get; set; }
        public DbSet<Session> Session { get; set; }
        
        public DbSet<SessionRuns> SessionRuns { get; set; }
        
        public DbSet<LatencyMeasurements> LatencyMeasurements { get; set; }




    }
}
