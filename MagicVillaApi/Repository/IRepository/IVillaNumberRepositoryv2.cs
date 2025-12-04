using MagicVillaApi.Models;

namespace MagicVillaApi.Repository.IRepository
{
    public interface IVillaNumberRepositoryv2: IRepository<VillaNumberv2>
    {
        Task<VillaNumberv2> UpdateAsync(VillaNumberv2 entity);
    }
}
