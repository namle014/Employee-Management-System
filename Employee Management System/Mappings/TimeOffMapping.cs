using AutoMapper;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class TimeOffMapping : Profile
    {
        public TimeOffMapping()
        {
            //Insert
            CreateMap<TimeOffCreateVModel, TimeOff>();
            // Update
            CreateMap<TimeOffUpdateVModel, TimeOff>();
            //Get All 
            CreateMap<TimeOff, TimeOffGetAllVModel>();
            //Get By Id
            CreateMap<TimeOff, TimeOffGetByIdVModel>();
            //Export
            CreateMap<TimeOff, TimeOffExportVModel>();
        }
        
    }
}
