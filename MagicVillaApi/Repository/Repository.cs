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
        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, int pageSize = 0, int pageNumber = 1)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (pageSize > 0)
            {
                if (pageSize > 100)
                {
                    pageSize = 100;
                }
                //skip0.take(5)
                //page number- 2     || page size -5
                //skip(5*(1)) take(5)
                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

                return await query.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includeProperties = null)
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

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

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
