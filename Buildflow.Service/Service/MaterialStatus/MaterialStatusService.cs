using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Service.Service.MaterialStatus
{
    public class MaterialStatusService
    {
        private readonly IMaterialStatusRepository _materialStatusRepository;

        public MaterialStatusService(IMaterialStatusRepository materialStatusRepository)
        {
            _materialStatusRepository = materialStatusRepository;
        }

        public async Task<List<MaterialStatusDto>> GetMaterialStatusAsync(int projectId)
        {
            return await _materialStatusRepository.GetMaterialStatusAsync(projectId);
        }
    }
}
