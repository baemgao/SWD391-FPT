﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Models.Response.StaffResponse;
using Services.Service;

namespace ManagerApartment.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private StaffService _staffService;
        public StaffController(StaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ResponseAccountStaff>>> GetAllInterviewers()
        {
            try
            {
                var interviewer = await _staffService.GetAllInterviewers();
                return Ok(interviewer);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<ResponseAccountStaff>> GetInterviewerById(Guid id)
        {
            try
            {
                var interviewer = await _staffService.GetInterviewerById(id);
                return Ok(interviewer);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }

}