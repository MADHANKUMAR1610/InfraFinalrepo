using Buildflow.Library.Repository.Interfaces;
using Buildflow.Library.UOW;
using Buildflow.Utility.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<List<MaterialDto>> GetMaterialAsync(int projectId)
        {
            return await _materialRepository.GetMaterialAsync(projectId);
        }
    }

}

