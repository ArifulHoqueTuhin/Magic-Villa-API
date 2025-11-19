using MagicVillaApi.Data;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.IRepository;

namespace MagicVillaApi.Repository
{
    public class VillaNumberRepositoryv2: Repository<VillaNumberv2>,IVillaNumberRepositoryv2
    {
  
        
            private readonly MagicVillaContext _dbData;

            public VillaNumberRepositoryv2(MagicVillaContext dbData) : base(dbData)
            {
                _dbData = dbData;
            }


            public async Task<VillaNumberv2> UpdateAsync(VillaNumberv2 entity)
            {
                //entity.UpdateDate = DateTime.Now;

                _dbData.VillaNumberv2s.Update(entity);
                await _dbData.SaveChangesAsync();
                return entity;
            }
        }

    }

