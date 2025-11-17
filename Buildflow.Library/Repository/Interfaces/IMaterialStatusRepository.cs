using Buildflow.Utility.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Buildflow.Library.Repository.Interfaces
{
    public interface IMaterialStatusRepository
    {
        Task<List<MaterialStatusDto>> GetMaterialStatusAsync(int projectId);
        Task<List<MaterialStatusDto>> TriggerRecalculationIfNeededAsync(int projectId);
    }
}
