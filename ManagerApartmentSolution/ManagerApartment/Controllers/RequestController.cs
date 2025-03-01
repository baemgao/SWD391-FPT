﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Models.Response.PackageResponse;
using Services.Models.Response.RequestRespponse;
using Services.Servicesss;

namespace ManagerApartment.Controllers
{
    [Authorize]
    [Route("api/request")]
    [ApiController]
    public class RequestController : Controller
    {
        private RequestService _requestService;
        public RequestController(RequestService requestService)
        {
            _requestService = requestService;
        }
        [HttpGet]
        public async Task<ActionResult<List<ResponseOfRequest>>> GetRequests(int page = 1, int pageSize = 10, string sortOrder = "asc")
        {
            try
            {
                var requests = await _requestService.GetAllRequests(page, pageSize, sortOrder);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseOfRequest>> GetRequestById(int id)
        {
            try
            {
                var request = await _requestService.GetRequestById(id);
                return Ok(request);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRequest(int id)
        {
            var deletedRequest = _requestService.DeleteRequest(id);
            return deletedRequest == null ? NoContent() : Ok(deletedRequest);
        }
    }
}
