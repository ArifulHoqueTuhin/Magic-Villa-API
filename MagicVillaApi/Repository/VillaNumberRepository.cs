using MagicVillaApi.Data;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.IRepository;

namespace MagicVillaApi.Repository
{
    public class VillaNumberRepository: Repository<VillaNumber>, IVillaNumberRepository
    {
    
        private readonly MagicVillaContext _dbData;
        public VillaNumberRepository(MagicVillaContext dbData) : base(dbData)
        {
            _dbData = dbData;
        }

        public async Task<VillaNumber> UpdateAsync(VillaNumber entity)
        {        
            _dbData.VillaNumbers.Update(entity);
            await _dbData.SaveChangesAsync();
            return entity;
        }
    }
}
