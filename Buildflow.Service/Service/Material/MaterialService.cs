using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buildflow.Service.Service.Material
{
    public class MaterialService
    {
        private readonly IMaterialRepository _materialRepository;

        public MaterialService(IMaterialRepository materialRepository)
        {
            _materialRepository = materialRepository;
        }

        // Only method needed now
        public async Task<List<MaterialDto>> GetMaterialAsync(int projectId)
        {
            return await _materialRepository.GetMaterialAsync(projectId);
        }
        public async Task<List<MaterialStatusDto>> GetMaterialStatusAsync(int projectId)
        {
            return await _materialRepository.GetMaterialStatusAsync(projectId);
        }
        public async Task<List<string>> GetMaterialNamesAsync(int projectId)
        {
            return await _materialRepository.GetAllMaterialNamesAsync(projectId);
        }

    }
}
