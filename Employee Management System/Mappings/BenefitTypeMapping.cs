﻿using AutoMapper;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class BenefitTypeMapping : Profile
    {
        public BenefitTypeMapping()
        {
            //Insert
            //CreateMap<BenefitCreateVModel, Benefit>();
            // Update
           // CreateMap<BenefitUpdateVModel, Benefit>();
            //Get All 
            CreateMap<BenefitType, BenefitTypeGetAllVModel>();
            //Get By Id
          //  CreateMap<Benefit, BenefitGetByIdVModel>();
            //Export
            //CreateMap<Benefit, BenefitExportVModel>();
        }
    }
}
