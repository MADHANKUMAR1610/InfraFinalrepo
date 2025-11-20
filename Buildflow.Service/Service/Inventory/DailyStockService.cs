using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buildflow.Service.Service.Inventory
{
    public class DailyStockService
    {
        private readonly IDailyStockRepository _dailyStockRepository;

        public DailyStockService(IDailyStockRepository dailyStockRepository)
        {
            _dailyStockRepository = dailyStockRepository
                ?? throw new ArgumentNullException(nameof(dailyStockRepository));
        }

        // 1️⃣ Reset daily stock at the start of the day
        public async Task ResetDailyStockAsync(int projectId)
        {
            if (projectId <= 0)
                throw new ArgumentException("Invalid project ID.", nameof(projectId));

            await _dailyStockRepository.ResetDailyStockAsync(projectId);
        }

        // 2️⃣ Update stock when inward/outward happens
        public async Task UpdateDailyStockAsync(
            int projectId,
            string itemName,
            decimal outwardQty = 0,
            decimal inwardQty = 0)
        {
            if (projectId <= 0)
                throw new ArgumentException("Invalid project ID.", nameof(projectId));

            if (string.IsNullOrWhiteSpace(itemName))
                throw new ArgumentException("Item name cannot be empty.", nameof(itemName));

            await _dailyStockRepository.UpdateDailyStockAsync(
                projectId,
                itemName,
                outwardQty,
                inwardQty);
        }

      
        // 4️⃣ Bulk update (when multiple movements happened)
        public async Task UpdateDailyStockForProjectAsync(int projectId)
        {
            if (projectId <= 0)
                throw new ArgumentException("Invalid project ID.", nameof(projectId));

            await _dailyStockRepository.UpdateDailyStockForProjectAsync(projectId);
        }

        // 5️⃣ Get today’s daily stock values for dashboard/material page
        public async Task<List<(string ItemName, decimal RemainingQty)>> GetDailyStockAsync(int projectId)
        {
            if (projectId <= 0)
                throw new ArgumentException("Invalid project ID.", nameof(projectId));

            return await _dailyStockRepository.GetDailyStockAsync(projectId);
        }
        public async Task AddNewBoqItemsToDailyStockAsync(int projectId, int boqId)
        {
            if (projectId <= 0)
                throw new ArgumentException("Invalid project ID.", nameof(projectId));

            if (boqId <= 0)
                throw new ArgumentException("Invalid BOQ ID.", nameof(boqId));

            await _dailyStockRepository.AddNewBoqItemsToDailyStockAsync(projectId, boqId);
        }

    }
}
