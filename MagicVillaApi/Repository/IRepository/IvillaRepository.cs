using System.Linq.Expressions;
using MagicVillaApi.Models;

namespace MagicVillaApi.Repository.IRepository
{
    public interface IvillaRepository : IRepository<VillaList>
    {


        Task <VillaList> UpdateAsync (VillaList entity);


    }
}
