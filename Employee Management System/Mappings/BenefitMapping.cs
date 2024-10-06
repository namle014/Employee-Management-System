using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class BenefitMapping : Profile
    {
        public BenefitMapping()
        {
            //Insert
            CreateMap<BenefitVModel, Benefit>();
            // Update
            CreateMap<BenefitUpdateVModel, Benefit>();
            //Get All 
            CreateMap<Benefit, BenefitGetAllVModel>();
            //Get By Id
            CreateMap<Benefit, BenefitGetByIdVModel>();
            //Export
            CreateMap<Benefit, BenefitExportVModel>();
        }
    }
}
