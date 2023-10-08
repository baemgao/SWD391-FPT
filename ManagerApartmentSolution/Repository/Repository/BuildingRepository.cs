﻿using ManagerApartment.Models;
using Repository.GenericRepository;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class BuildingRepository : GenericRepository<Building>, IBuildingRepository
    {
        public BuildingRepository (ManagerApartmentContext context) : base(context) { }
        public async Task<List<Building>> GetAllBuildings()
        {
            var buildings = await _context.Buildings
                .ToListAsync();
            return buildings;
        }

        public async Task<Building> GetBuildingById(int id)
        {
            return await _context.Buildings.FirstOrDefaultAsync(r => r.BuildingId == id);
        }
    }
}
