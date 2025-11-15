using Buildflow.Utility.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository.Interfaces
{
    public interface IMaterialRepository
    {
        Task<List<MaterialDto>> TriggerRecalculationIfNeededAsync(int projectId);
        Task<List<MaterialDto>> GetMaterialAsync(int projectId);


    }
}
