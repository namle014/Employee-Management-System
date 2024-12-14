using AutoMapper;
using OA.Core.VModels;
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
