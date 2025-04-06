using System.Linq.Expressions;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MagicVillaApi.Repository
{
    public class VillaRepository : Repository<VillaList>, IvillaRepository
    {
        private readonly MagicVillaContext _dbData;

        public VillaRepository(MagicVillaContext dbData) :base(dbData) 
        {
            _dbData = dbData;
        }


        public async Task<VillaList> UpdateAsync(VillaList entity)
        {
            entity.UpdateDate = DateTime.Now;

            _dbData.VillaLists.Update(entity);
            await _dbData.SaveChangesAsync();
            return entity;
        }




    }
}
