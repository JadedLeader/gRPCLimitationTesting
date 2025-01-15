using ConfigurationStuff.DbContexts;
using ConfigurationStuff.DbModels;
using DbManagerWorkerService.Interfaces.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationStuff.Abstracts
{
    public abstract class RepositoryAbstract<T> where T : class
    {
        private DataContexts _context;
        protected RepositoryAbstract(DataContexts context)
        {
            _context = context;
        }

        public virtual async Task<T> AddToDbAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            return entity;
        }
        public virtual async Task RemoveRangeAsync(List<ClientInstance> clientList)
        {
            // Ensure that _context.ClientInstances is valid
            if (_context.ClientInstance == null)
            {
                throw new InvalidOperationException("ClientInstances DbSet is not available.");
            }

            foreach (var clientInstance in clientList)
            {
                var dbEntry = _context.Entry(clientInstance);

                // Set RowVersion in OriginalValues to handle concurrency
                if (dbEntry.Property("RowVersion") != null)
                {
                    dbEntry.OriginalValues["RowVersion"] = clientInstance.RowVersion;
                }
            }

            // Remove the client instances in bulk
            _context.ClientInstance.RemoveRange(clientList);
            await _context.SaveChangesAsync();

        }

        public virtual async Task ReloadAsync(T entity)
        {
            await _context.Entry(entity).ReloadAsync();
        }

        public virtual async Task<T> RemoveFromDbAsync(T entity)
        {
            _context.Set<T>().Remove(entity);

            return entity;
        }

        public virtual async Task<T> UpdateDbAsync(T entity)
        {
            _context.Set<T>().Update(entity);

            return entity;
        }

        public virtual async Task<IEnumerable<T>> GetDbContent()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public abstract Task<T> GetRecordViaId(Guid? recordId);

        public virtual async Task SaveAsync()
        {
            try
            {
                Console.WriteLine($"SaveAsync called on DbContext instance: {_context.GetHashCode()}");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // EF found 0 rows matching the PK + rowversion => concurrency conflict or row not found
                /* var entry = ex.Entries.Single();
                 var dbValues = entry.GetDatabaseValues();

                 if (dbValues == null)
                 {
                     // That means EF truly couldn't find the row (it was deleted or rowversion mismatched).
                     throw new Exception("The Session has been removed from the database by another process.");
                 }
                 else
                 {
                     // The row still exists, but the rowversion changed => concurrency conflict
                     // Decide if you want to overwrite the DB or discard local changes.

                     // Example: discard local changes, keep DB:
                     entry.CurrentValues.SetValues(dbValues);

                     // Retry once, or handle differently if you want a more robust retry strategy.
                     await _context.SaveChangesAsync();
                 } */

                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is Session session)
                    {
                        var databaseValues = await entry.GetDatabaseValuesAsync();
                        if (databaseValues == null)
                        {
                            throw new Exception("The Session has been deleted by another process.");
                        }

                        // Resolve the conflict by overwriting with database values
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                }

                // Retry saving
                await _context.SaveChangesAsync();
            }
        }

    }
}
