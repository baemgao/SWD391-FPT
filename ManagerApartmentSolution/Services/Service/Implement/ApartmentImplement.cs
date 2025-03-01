﻿using AutoMapper;
using Services.Interfaces;
using Services.Interfaces.IUnitOfWork;
using Services.Models.Response.ApartmentResponse;
using Services.Models.Response.TennantResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Servicesss.Implement
{
    public class ApartmentImplement : ApartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IApartmentRepository _apartmentRepository;
        public ApartmentImplement(IUnitOfWork unitOfWork, IMapper mapper, IApartmentRepository apartmentRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _apartmentRepository = apartmentRepository;
        }
        public async Task<List<ResponseOfApartment>> GetAllApartments(int page, int pageSize, string sortOrder)
        {
            var apartments = await _unitOfWork.Apartment.GetAllApartments();
            if (apartments is null)
            {
                throw new Exception("The apartment list is empty");
            }
            var apartmentDTO = _mapper.Map<List<ResponseOfApartment>>(apartments);
            // Sắp xếp danh sách yêu cầu theo FromDate gần nhất
            if (sortOrder == "desc")
            {
                apartmentDTO = apartmentDTO.OrderByDescending(r => r.FromDate).ToList();
            }
            else
            {
                apartmentDTO = apartmentDTO.OrderBy(r => r.FromDate).ToList();
            }

            var startIndex = (page - 1) * pageSize;
            var pagedApartments = apartmentDTO.Skip(startIndex).Take(pageSize).ToList();

            return pagedApartments;
        }

        public async Task<ResponseOfApartment> GetApartmentById(int id)
        {
            var apartment = await _apartmentRepository.GetApartmentById(id);
            if (apartment is null)
            {
                throw new Exception("Can not found by " + id);
            }
            return _mapper.Map<ResponseOfApartment>(apartment);
        }
    }
}
