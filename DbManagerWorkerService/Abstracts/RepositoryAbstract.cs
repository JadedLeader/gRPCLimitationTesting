using DbManagerWorkerService.Interfaces.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DbManagerWorkerService.Abstracts
{
    public abstract class RepositoryAbstract<T> where T : class
    {
        private DbContext _context;
        protected RepositoryAbstract(DbContext context)
        {
            _context = context;
        }

        public virtual async Task<T> AddToDbAsync(T entity)
        {
            _context.Set<T>().Add(entity);

            await _context.SaveChangesAsync();

            return entity;
        }

        public virtual async Task<T> RemoveFromDbAsync(T entity)
        {
            _context.Set<T>().Remove(entity);

            await _context.SaveChangesAsync();

            return entity;
        }

        public virtual async Task<T> UpdateDbAsync(T entity)
        {
            _context.Set<T>().Update(entity);

            await _context.SaveChangesAsync();

            return entity;
        }

        public virtual async Task<IEnumerable<T>> GetDbContent()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public abstract Task<T> GetRecordViaId(Guid? recordId);

    }
}
