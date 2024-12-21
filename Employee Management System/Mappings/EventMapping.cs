using AutoMapper;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class EventMapping : Profile
    {
        public EventMapping()
        {
            //Insert
            CreateMap<EventCreateVModel, Events>();
            // Update
            CreateMap<EventUpdateVModel, Events>();
            //Get All 
            CreateMap<Events, EventGetAllVModel>();
            //Get By Id
            CreateMap<Events, EventGetByIdVModel>();
            //Export
            CreateMap<Events, EventExportVModel>();
        }
    }
}
