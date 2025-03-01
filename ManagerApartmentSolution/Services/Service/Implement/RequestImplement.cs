﻿using AutoMapper;
using Domain.Enums.Status;
using ManagerApartment.Models;
using Services.Interfaces.IUnitOfWork;
using Services.Models.Response.RequestRespponse;
using Services.Models.Response.TennantResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Servicesss.Implement
{
    public class RequestImplement : RequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public RequestImplement(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task DeleteRequest(int requestId)
        {
            var request = _unitOfWork.Request.GetById(requestId);
            if (request is null)
            {
                throw new Exception("Can not found by" + requestId);
            }
            request.ReqStatus = RequestEnum.NOTPROCESSED.ToString();
            _unitOfWork.Request.Update(request);
            _unitOfWork.Save();
        }

        public async Task<List<ResponseOfRequest>> GetAllRequests(int page, int pageSize, string sortOrder)
        {
            var rqs = await _unitOfWork.Request.GetAllRequests();
            if (rqs is null)
            {
                throw new Exception("The request list is empty");
            }
            var requestDtos = _mapper.Map<List<ResponseOfRequest>>(rqs);

            // Sắp xếp danh sách yêu cầu theo BookDateTime gần nhất
            if (sortOrder == "desc")
            {
                requestDtos = requestDtos.OrderByDescending(r => r.BookDateTime).ToList();
            }
            else
            {
                requestDtos = requestDtos.OrderBy(r => r.BookDateTime).ToList();
            }

            var startIndex = (page - 1) * pageSize;
            var pagedRequests = requestDtos.Skip(startIndex).Take(pageSize).ToList();

            return pagedRequests;
        }

        public async Task<ResponseOfRequest> GetRequestById(int id)
        {
            var rq = await _unitOfWork.Request.GetRequestById(id);
            if (rq is null)
            {
                throw new Exception("The request does not exist");
            }
            return _mapper.Map<ResponseOfRequest>(rq);
        }

    }
}
