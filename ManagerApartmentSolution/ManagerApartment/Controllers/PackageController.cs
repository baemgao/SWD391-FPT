﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Models.Response.PackageResponse;
using Services.Servicesss;

namespace ManagerApartment.Controllers
{
    [Authorize]
    [Route("api/package")]
    [ApiController]
    public class PackageController : Controller
    {
        private PackageService _packageService;
        public PackageController(PackageService packageService)
        {
            _packageService = packageService;
        }
        [HttpGet]
        public async Task<ActionResult<List<ResponseOfPackage>>> GetPackages(int page = 1, int pageSize = 10, string sortOrder = "asc")
        {
            try
            {
                var packages = await _packageService.GetAllPackages(page, pageSize, sortOrder);
                return Ok(packages);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseOfPackage>> GetPackageById(int id)
        {
            try
            {
                var package = await _packageService.GetPackageById(id);
                return Ok(package);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
