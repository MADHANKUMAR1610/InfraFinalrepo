using Buildflow.Utility.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository.Interfaces
{
    public interface IMaterialRepository
    {
        // MAIN MATERIAL GET
        Task<List<MaterialDto>> GetMaterialAsync(int projectId);
        Task<IEnumerable<object>> GetMaterialSummaryAsync(int projectId);
        Task<List<string>> GetAllMaterialNamesAsync(int projectId);


    }
}
