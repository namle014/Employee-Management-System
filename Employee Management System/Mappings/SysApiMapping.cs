using AutoMapper;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class SysApiMapping : Profile
    {
        public SysApiMapping()
        {
            //Insert
            CreateMap<SysApiCreateVModel, SysApi>();
            // Update
            CreateMap<SysApiUpdateVModel, SysApi>();
            //Get All 
            CreateMap<SysApi, SysApiGetAllVModel>();
            //Get By Id
            CreateMap<SysApi, SysApiGetByIdVModel>();
            //Export
            CreateMap<SysApi, SysApiExportVModel>();
        }
    }
}
