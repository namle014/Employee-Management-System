using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class HolidayMapping : Profile
    {
        public HolidayMapping()
        {
            //Insert
            CreateMap<HolidayCreateVModel, Holiday>();
            // Update
            CreateMap<HolidayUpdateVModel, Holiday>();
            //Get All 
            CreateMap<Holiday, HolidayGetAllVModel>();
            //Get By Id
            CreateMap<Holiday, HolidayGetByIdVModel>();
            //Export
            CreateMap<Holiday, HolidayExportVModel>();
        }
    }
}
