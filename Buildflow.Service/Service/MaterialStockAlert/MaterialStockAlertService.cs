using Buildflow.Library.Repository;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Service.Service.MaterialStockAlert
{
   
        public class MaterialStockAlertService
        {
            private readonly IMaterialStockAlertRepository _repository;

            public MaterialStockAlertService(IMaterialStockAlertRepository repository)
            {
                _repository = repository;
            }

            public async Task<List<MaterialDto>> GetMaterialStockAlertsAsync(int projectId)
            {
                return await _repository.GetMaterialStockAlertsAsync(projectId);
            }
        }
    }

