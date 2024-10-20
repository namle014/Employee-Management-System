using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class InsuranceMapping : Profile
    {
        public InsuranceMapping()
        {
            //Insert
            CreateMap<InsuranceCreateVModel, Insurance>();
            // Update
            CreateMap<InsuranceUpdateVModel, Insurance>();
            //Get All 
            CreateMap<Insurance, InsuranceGetAllVModel>();
            //Get By Id
            CreateMap<Insurance, InsuranceGetByIdVModel>();
            //Export
            CreateMap<Insurance, InsuranceExportVModel>();
        }
    }
}
