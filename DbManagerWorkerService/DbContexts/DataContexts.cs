using DbManagerWorkerService.DbModels;
using DbManagerWorkerService.Interfaces.DataContext;
using Microsoft.EntityFrameworkCore;

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
            var commDelay = modelBuilder.Entity<CommunicationDelay>();

            //key
            commDelay.HasKey(k => k.Id);

            //props
            commDelay.Property(dg => dg.DelayGuid).IsRequired();
            commDelay.Property(ct => ct.CommunicationType).IsRequired();
            commDelay.Property(dl => dl.DataLength).IsRequired();
            commDelay.Property(d => d.Delay).IsRequired();
         
        }

        public async Task<int> SaveChangesAsync()
        {
             return await base.SaveChangesAsync();
        }

        public DbSet<CommunicationDelay> CommunicationDelay { get; set; }

    }
}
