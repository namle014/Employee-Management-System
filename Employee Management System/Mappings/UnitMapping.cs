using AutoMapper;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class UnitMapping : Profile
    {
        public UnitMapping()
        {
            //Insert
            CreateMap<UnitCreateVModel, Unit>();
            // Update
            CreateMap<UnitUpdateVModel, Unit>();
            //Get All 
            CreateMap<Unit, UnitGetAllVModel>();
            //Get By Id
            CreateMap<Unit, UnitGetByIdVModel>();
            //Get list
            CreateMap<Unit, UnitExportVModel>();
        }
    }
}
