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
    public class ContractDetailRepository : GenericRepository<ContractDetail>, IContractDetailRepository
    {
        public ContractDetailRepository(ManagerApartmentContext context) : base(context) { }
        public async Task<List<ContractDetail>> GetAllContractDetails()
        {
            var contractDetails = await _context.ContractDetails
                .ToListAsync();
            return contractDetails;
        }

        public async Task<ContractDetail> GetContractDetailById(int id)
        {
            return await _context.ContractDetails.FirstOrDefaultAsync(r => r.ContractDetailId == id);
        }
    }
}
