using MagicVillaApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using MagicVillaApi.Repository.IRepository;

namespace MagicVillaApi.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly MagicVillaContext _dbData;
        internal DbSet<T> _dbSet;

        public Repository(MagicVillaContext dbData)
        {
            _dbData = dbData;
            _dbSet = _dbData.Set<T>();
            
        }
        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true)
        {
            IQueryable<T> query = _dbSet;

            if (!tracked)

            {
                query = query.AsNoTracking();
            }


            if (filter != null)
            {
                query = query.Where(filter);
            }

            //return await query.FirstOrDefaultAsync();

            return await query.FirstOrDefaultAsync() ?? default!;



        }


        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveAsync();
        }


        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await SaveAsync();
        }


        public async Task SaveAsync()
        {
            await _dbData.SaveChangesAsync();
        }
    }
}
