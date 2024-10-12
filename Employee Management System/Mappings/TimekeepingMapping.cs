using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class TimekeepingMapping : Profile
    {
        public TimekeepingMapping()
        {
            //Insert
            CreateMap<TimekeepingCreateVModel, Timekeeping>();
            // Update
            CreateMap<TimekeepingUpdateVModel, Timekeeping>();
            //Get All 
            CreateMap<Timekeeping, TimekeepingGetAllVModel>();
            //Get By Id
            CreateMap<Timekeeping, TimekeepingGetByIdVModel>();
            //Export
            CreateMap<Timekeeping, TimekeepingExportVModel>();
        }
    }
}
