﻿using AutoMapper;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebApi.Mappings
{
    public class SysConfigurationMapping : Profile
    {
        public SysConfigurationMapping()
        {
            //Insert
            CreateMap<SysConfigurationCreateVModel, SysConfiguration>();
            // Update
            CreateMap<SysConfigurationUpdateVModel, SysConfiguration>();
            //Get All 
            CreateMap<SysConfiguration, SysConfigurationGetAllVModel>();
                //.ForMember(desc => desc.CreatedDate, src => src.MapFrom(x => x.CreatedDate.Value.ToString("yyyy-MM-dd h:mm")));
            //Get By Id
            CreateMap<SysConfiguration, SysConfigurationGetByIdVModel>();
            CreateMap<SysConfiguration, SysConfigurationExportVModel>();
        }
    }
}
